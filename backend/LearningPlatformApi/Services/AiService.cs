using OpenAI.Chat;

namespace LearningPlatformApi.Services
{
    public class AiService
    {
        private readonly ChatClient _client;

        public AiService(string apiKey)
        {
            _client = new ChatClient("gpt-4o-mini", apiKey);
        }

        public async Task<string> GenerateLesson(string category, string subCategory, string userPrompt)
        {
            var prompt =
                "You are an expert learning coach. Create clear, structured, beginner-friendly lessons " +
                "with headings, bullet points, and a short summary.\n\n" +
                $"Topic category: {category}\n" +
                $"Sub-topic: {subCategory}\n" +
                $"Student request: {userPrompt}\n\n" +
                "Generate a concise but high-quality learning lesson.";

            ChatCompletion completion = await _client.CompleteChatAsync(prompt);
            return completion.Content[0].Text;
        }
    }
}
