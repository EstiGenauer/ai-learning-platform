using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace LearningPlatformApi.Services
{
    public class AiService
    {
        private readonly ChatClient _client;

        public AiService(string apiKey)
        {
            // כאן נגדיר את החיבור ל-OpenAI
            _client = new ChatClient("gpt-4o", apiKey);
        }

        public async Task<string> GetAiResponse(string userPrompt)
        {
            ChatCompletion completion = await _client.CompleteChatAsync(userPrompt);
            return completion.Content[0].Text;
        }
    }
}