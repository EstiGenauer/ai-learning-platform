using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.DTOs;
using LearningPlatformApi.Models;
using LearningPlatformApi.Services;
using System.Security.Claims;

namespace LearningPlatformApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromptsController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly ILogger<PromptsController> _logger;
        private readonly AppDbContext _context;

        public PromptsController(IAiService aiService, ILogger<PromptsController> logger, AppDbContext context)
        {
            _aiService = aiService;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<PromptDto>> Create([FromBody] CreatePromptRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var category = await _context.Categories.FindAsync(request.CategoryId);
            var subCategory = await _context.SubCategories
                .FirstOrDefaultAsync(s => s.Id == request.SubCategoryId && s.CategoryId == request.CategoryId);

            if (category == null || subCategory == null)
                return BadRequest(new { message = "Invalid category or sub-category selection." });

            try
            {
                _logger.LogInformation("AI request from user {UserId}: {Prompt}", userId, request.PromptText);

                var aiResponse = await _aiService.GenerateLesson(
                    category.Name,
                    subCategory.Name,
                    request.PromptText);

                var prompt = new Prompt
                {
                    UserId = userId,
                    CategoryId = request.CategoryId,
                    SubCategoryId = request.SubCategoryId,
                    PromptText = request.PromptText,
                    Response = aiResponse,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Prompts.Add(prompt);
                await _context.SaveChangesAsync();

                return Ok(MapToDto(prompt, category.Name, subCategory.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate AI lesson");
                var message = GetAiErrorMessage(ex);
                return StatusCode(500, new { message });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<PromptDto>>> GetMyHistory()
        {
            var userId = GetUserId();
            var prompts = await _context.Prompts
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return prompts.Select(p => MapToDto(
                p,
                p.Category?.Name ?? "",
                p.SubCategory?.Name ?? "",
                p.User?.Name ?? "")).ToList();
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException());

        private static string GetAiErrorMessage(Exception ex)
        {
            var detail = ex.ToString();

            if (detail.Contains("API key is not configured", StringComparison.OrdinalIgnoreCase))
                return "OpenAI API key is missing. Add OPENAI_API_KEY to .env and restart the backend.";

            if (detail.Contains("model_not_found", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("does not have access to model", StringComparison.OrdinalIgnoreCase))
                return "Your OpenAI project does not have access to the configured model. Add OPENAI_MODEL=gpt-4o to .env and run: docker compose up -d --build backend";

            if (detail.Contains("401", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("invalid_api_key", StringComparison.OrdinalIgnoreCase))
                return "Invalid OpenAI API key. Check OPENAI_API_KEY in .env.";

            if (detail.Contains("429", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("insufficient_quota", StringComparison.OrdinalIgnoreCase))
                return "OpenAI quota exceeded. Check billing on platform.openai.com.";

            return "AI service error. Please try again later.";
        }

        private static PromptDto MapToDto(Prompt prompt, string categoryName, string subCategoryName, string? userName = null) =>
            new()
            {
                Id = prompt.Id,
                UserId = prompt.UserId,
                UserName = userName ?? "",
                CategoryName = categoryName,
                SubCategoryName = subCategoryName,
                PromptText = prompt.PromptText,
                Response = prompt.Response,
                CreatedAt = prompt.CreatedAt
            };
    }
}
