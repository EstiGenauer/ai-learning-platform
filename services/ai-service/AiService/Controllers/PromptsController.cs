using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiService.Data;
using AiService.DTOs;
using AiService.Models;
using AiService.Services;
using System.Security.Claims;

namespace AiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromptsController : ControllerBase
    {
        private readonly ILessonGenerator _aiService;
        private readonly ILogger<PromptsController> _logger;
        private readonly PromptsDbContext _context;
        private readonly CatalogClient _catalog;

        public PromptsController(
            ILessonGenerator aiService,
            ILogger<PromptsController> logger,
            PromptsDbContext context,
            CatalogClient catalog)
        {
            _aiService = aiService;
            _logger = logger;
            _context = context;
            _catalog = catalog;
        }

        [HttpPost]
        public async Task<ActionResult<PromptDto>> Create([FromBody] CreatePromptRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";

            CategorySelectionDto? selection;
            try
            {
                selection = await _catalog.ValidateSelectionAsync(request.CategoryId, request.SubCategoryId);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message });
            }

            if (selection == null)
                return BadRequest(new { message = "Invalid category or sub-category selection." });

            try
            {
                _logger.LogInformation("AI request from user {UserId}: {Prompt}", userId, request.PromptText);

                var aiResponse = await _aiService.GenerateLesson(
                    selection.CategoryName,
                    selection.SubCategoryName,
                    request.PromptText);

                var prompt = new Prompt
                {
                    UserId = userId,
                    UserName = userName,
                    CategoryId = selection.CategoryId,
                    CategoryName = selection.CategoryName,
                    SubCategoryId = selection.SubCategoryId,
                    SubCategoryName = selection.SubCategoryName,
                    PromptText = request.PromptText,
                    Response = aiResponse,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Prompts.Add(prompt);
                await _context.SaveChangesAsync();

                return Ok(MapToDto(prompt));
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
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return prompts.Select(MapToDto).ToList();
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
                return "Your OpenAI project does not have access to the configured model. Add OPENAI_MODEL=gpt-4o to .env and run: docker compose up -d --build ai-service";

            if (detail.Contains("401", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("invalid_api_key", StringComparison.OrdinalIgnoreCase))
                return "Invalid OpenAI API key. Check OPENAI_API_KEY in .env.";

            if (detail.Contains("429", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("insufficient_quota", StringComparison.OrdinalIgnoreCase))
                return "OpenAI quota exceeded. Check billing on platform.openai.com.";

            return "AI service error. Please try again later.";
        }

        private static PromptDto MapToDto(Prompt prompt) =>
            new()
            {
                Id = prompt.Id,
                UserId = prompt.UserId,
                UserName = prompt.UserName,
                CategoryName = prompt.CategoryName,
                SubCategoryName = prompt.SubCategoryName,
                PromptText = prompt.PromptText,
                Response = prompt.Response,
                CreatedAt = prompt.CreatedAt
            };
    }
}
