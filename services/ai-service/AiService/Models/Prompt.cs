namespace AiService.Models
{
    /// <summary>
    /// Owned by the AI service. Because each service has its own database, this record cannot
    /// hold foreign keys to users/categories in other services. Instead it stores the display
    /// names denormalized at creation time (a deliberate microservices data-ownership trade-off).
    /// </summary>
    public class Prompt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
