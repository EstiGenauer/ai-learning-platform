using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiService.Data;
using AiService.DTOs;

namespace AiService.Controllers
{
    [ApiController]
    [Route("api/Admin/prompts")]
    [Authorize(Roles = "Admin")]
    public class AdminPromptsController : ControllerBase
    {
        private readonly PromptsDbContext _context;

        public AdminPromptsController(PromptsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<PromptDto>>> GetAllPrompts()
        {
            var prompts = await _context.Prompts
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PromptDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = p.UserName,
                    CategoryName = p.CategoryName,
                    SubCategoryName = p.SubCategoryName,
                    PromptText = p.PromptText,
                    Response = p.Response,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(prompts);
        }
    }
}
