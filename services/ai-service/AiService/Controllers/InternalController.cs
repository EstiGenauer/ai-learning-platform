using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiService.Data;

namespace AiService.Controllers
{
    /// <summary>
    /// Internal (service-to-service) endpoints, not exposed through the public API gateway.
    /// Consumed by the Auth service (prompt counts per user) and the Catalog service
    /// (whether a category/sub-category is still referenced before deletion).
    /// </summary>
    [ApiController]
    [Route("internal")]
    public class InternalController : ControllerBase
    {
        private readonly PromptsDbContext _context;

        public InternalController(PromptsDbContext context)
        {
            _context = context;
        }

        [HttpGet("prompt-counts")]
        public async Task<ActionResult<Dictionary<int, int>>> GetPromptCounts()
        {
            var counts = await _context.Prompts
                .GroupBy(p => p.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(counts.ToDictionary(c => c.UserId, c => c.Count));
        }

        [HttpGet("prompt-usage")]
        public async Task<ActionResult> GetPromptUsage(
            [FromQuery] int? categoryId, [FromQuery] int? subCategoryId)
        {
            var inUse = false;

            if (categoryId.HasValue)
                inUse = await _context.Prompts.AnyAsync(p => p.CategoryId == categoryId.Value);
            else if (subCategoryId.HasValue)
                inUse = await _context.Prompts.AnyAsync(p => p.SubCategoryId == subCategoryId.Value);

            return Ok(new { inUse });
        }
    }
}
