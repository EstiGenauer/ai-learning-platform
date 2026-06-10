using LearningPlatformApi.Models;

namespace LearningPlatformApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SubCategory> SubCategories { get; set; } = new();
    }
}