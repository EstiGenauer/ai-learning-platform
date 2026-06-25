namespace AiService.Services
{
    public interface ILessonGenerator
    {
        Task<string> GenerateLesson(string category, string subCategory, string userPrompt);
    }
}
