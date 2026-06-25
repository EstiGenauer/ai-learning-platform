using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.DTOs;

namespace CatalogService.Controllers
{
    /// <summary>
    /// Internal (service-to-service) endpoints. Not exposed through the public API gateway —
    /// reachable only from inside the cluster/compose network. Used by the AI service to
    /// validate a category/sub-category selection before generating a lesson.
    /// </summary>
    [ApiController]
    [Route("internal")]
    public class InternalController : ControllerBase
    {
        private readonly CatalogDbContext _context;

        public InternalController(CatalogDbContext context)
        {
            _context = context;
        }

        [HttpGet("validate")]
        public async Task<ActionResult<ValidateSelectionDto>> Validate(
            [FromQuery] int categoryId, [FromQuery] int subCategoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            var subCategory = await _context.SubCategories
                .FirstOrDefaultAsync(s => s.Id == subCategoryId && s.CategoryId == categoryId);

            if (category == null || subCategory == null)
                return NotFound(new { message = "Invalid category or sub-category selection." });

            return Ok(new ValidateSelectionDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                SubCategoryId = subCategory.Id,
                SubCategoryName = subCategory.Name
            });
        }
    }
}
