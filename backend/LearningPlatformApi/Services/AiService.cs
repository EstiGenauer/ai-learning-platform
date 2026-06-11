using OpenAI.Chat;

namespace LearningPlatformApi.Services
{
    public class AiService : IAiService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private ChatClient? _client;

        public AiService(string apiKey, string model)
        {
            _apiKey = apiKey ?? string.Empty;
            _model = string.IsNullOrWhiteSpace(model) ? "gpt-4o-mini" : model;
        }

        public async Task<string> GenerateLesson(string category, string subCategory, string userPrompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey == "dummy-key")
                throw new InvalidOperationException("OpenAI API key is not configured. Set OPENAI_API_KEY in .env and restart.");

            _client ??= new ChatClient(_model, _apiKey);
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
