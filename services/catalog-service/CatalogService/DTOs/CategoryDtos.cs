using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SubCategoryDto> SubCategories { get; set; } = new();
    }

    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    public class CreateCategoryRequest
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateSubCategoryRequest
    {
        [Required, MinLength(1)]
        public string Name { get; set; } = string.Empty;
    }

    public class ValidateSelectionDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
    }
}
