using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.DTOs;

namespace LearningPlatformApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.Id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.IsAdmin,
                    PromptCount = u.Prompts.Count
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("prompts")]
        public async Task<ActionResult<List<PromptDto>>> GetAllPrompts()
        {
            var prompts = await _context.Prompts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PromptDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = p.User!.Name,
                    CategoryName = p.Category!.Name,
                    SubCategoryName = p.SubCategory!.Name,
                    PromptText = p.PromptText,
                    Response = p.Response,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(prompts);
        }
    }
}
