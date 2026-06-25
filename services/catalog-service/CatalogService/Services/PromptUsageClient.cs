namespace CatalogService.Services
{
    /// <summary>
    /// Cross-service client: asks the AI service whether any prompts reference a given
    /// category or sub-category before allowing deletion. Replaces the cross-table foreign
    /// key check that existed in the monolith. If the AI service is unreachable the delete
    /// is allowed (fail-open) and a warning is logged.
    /// </summary>
    public class PromptUsageClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<PromptUsageClient> _logger;

        public PromptUsageClient(HttpClient http, ILogger<PromptUsageClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public Task<bool> CategoryInUseAsync(int categoryId) =>
            CheckAsync($"/internal/prompt-usage?categoryId={categoryId}");

        public Task<bool> SubCategoryInUseAsync(int subCategoryId) =>
            CheckAsync($"/internal/prompt-usage?subCategoryId={subCategoryId}");

        private async Task<bool> CheckAsync(string path)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<UsageResult>(path);
                return result?.InUse ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AI service unreachable for prompt-usage check; allowing delete.");
                return false;
            }
        }

        private sealed class UsageResult
        {
            public bool InUse { get; set; }
        }
    }
}
