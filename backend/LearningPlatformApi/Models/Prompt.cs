namespace LearningPlatformApi.Models
{
    public class Prompt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string PromptText { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Category? Category { get; set; }
        public SubCategory? SubCategory { get; set; }
    }
}
