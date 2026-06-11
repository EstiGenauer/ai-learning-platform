using System.ComponentModel.DataAnnotations;

namespace LearningPlatformApi.DTOs
{
    public class CreatePromptRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SubCategoryId { get; set; }

        [Required, MinLength(3)]
        public string PromptText { get; set; } = string.Empty;
    }

    public class PromptDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SubCategoryName { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
