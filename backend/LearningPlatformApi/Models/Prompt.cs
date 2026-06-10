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

        // ניווט לטבלאות קשורות (יעזור ל-EF ליצור קשרים נכונים)
        public User? User { get; set; }
    }
}
