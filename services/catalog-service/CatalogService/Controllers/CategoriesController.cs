using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.DTOs;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CatalogDbContext _context;

        public CategoriesController(CatalogDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    SubCategories = c.SubCategories
                        .OrderBy(s => s.Name)
                        .Select(s => new SubCategoryDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            CategoryId = s.CategoryId
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}
