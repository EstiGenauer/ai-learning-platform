namespace AuthService.Services
{
    /// <summary>
    /// Cross-service client: asks the AI service how many prompts each user has created.
    /// Demonstrates synchronous inter-service communication with graceful degradation —
    /// if the AI service is unreachable, prompt counts default to 0 instead of failing.
    /// </summary>
    public class PromptStatsClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<PromptStatsClient> _logger;

        public PromptStatsClient(HttpClient http, ILogger<PromptStatsClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<Dictionary<int, int>> GetPromptCountsAsync()
        {
            try
            {
                var counts = await _http.GetFromJsonAsync<Dictionary<int, int>>("/internal/prompt-counts");
                return counts ?? new Dictionary<int, int>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AI service unreachable for prompt counts; defaulting to 0.");
                return new Dictionary<int, int>();
            }
        }
    }
}
