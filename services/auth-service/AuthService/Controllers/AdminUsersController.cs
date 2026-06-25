using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/Admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IPromptStatsClient _promptStats;

        public AdminUsersController(AuthDbContext context, IPromptStatsClient promptStats)
        {
            _context = context;
            _promptStats = promptStats;
        }

        [HttpGet]
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
                    u.IsAdmin
                })
                .ToListAsync();

            var counts = await _promptStats.GetPromptCountsAsync();

            var result = users.Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Phone,
                u.IsAdmin,
                PromptCount = counts.TryGetValue(u.Id, out var c) ? c : 0
            });

            return Ok(result);
        }
    }
}
