namespace LearningPlatformApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // הוספנו את זה כדי שנוכל לאמת משתמשים ב-Login
        public string PasswordHash { get; set; } = string.Empty; 
        
        public List<Prompt> Prompts { get; set; } = new();
    }
}