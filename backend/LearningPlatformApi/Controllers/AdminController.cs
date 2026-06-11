using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.DTOs;
using LearningPlatformApi.Models;

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

        private ActionResult ValidationError()
        {
            var message = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return BadRequest(new { message = string.IsNullOrWhiteSpace(message) ? "Invalid input." : message });
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

        [HttpGet("categories")]
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

        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var name = request.Name.Trim();
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower()))
                return BadRequest(new { message = "Category with this name already exists." });

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new CategoryDto { Id = category.Id, Name = category.Name, SubCategories = new() });
        }

        [HttpPost("categories/{categoryId}/subcategories")]
        public async Task<ActionResult<SubCategoryDto>> CreateSubCategory(
            int categoryId,
            [FromBody] CreateSubCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            var name = request.Name.Trim();
            if (await _context.SubCategories.AnyAsync(s =>
                    s.CategoryId == categoryId && s.Name.ToLower() == name.ToLower()))
                return BadRequest(new { message = "Sub-category with this name already exists in this category." });

            var subCategory = new SubCategory { Name = name, CategoryId = categoryId };
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();

            return Ok(new SubCategoryDto
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                CategoryId = subCategory.CategoryId
            });
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var hasPrompts = await _context.Prompts.AnyAsync(p => p.CategoryId == id);
            if (hasPrompts)
                return BadRequest(new { message = "Cannot delete category that has learning prompts." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("subcategories/{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
                return NotFound(new { message = "Sub-category not found." });

            var hasPrompts = await _context.Prompts.AnyAsync(p => p.SubCategoryId == id);
            if (hasPrompts)
                return BadRequest(new { message = "Cannot delete sub-category that has learning prompts." });

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
