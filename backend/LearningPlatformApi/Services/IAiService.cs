namespace LearningPlatformApi.Services
{
    public interface IAiService
    {
        Task<string> GenerateLesson(string category, string subCategory, string userPrompt);
    }
}
