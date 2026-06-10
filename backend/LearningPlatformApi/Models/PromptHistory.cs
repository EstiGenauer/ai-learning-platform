namespace LearningPlatformApi.Models
{
    public class PromptHistory
    {
        public int Id { get; set; }
        public string? Question { get; set; } // הוספתי סימן שאלה ?
        public string? Answer { get; set; }   // הוספתי סימן שאלה ?
        public DateTime CreatedAt { get; set; } 
    }
}