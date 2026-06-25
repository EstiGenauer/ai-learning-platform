using AiService.DTOs;

namespace AiService.Services
{
    /// <summary>
    /// Cross-service client: validates a category/sub-category selection against the Catalog
    /// service before a lesson is generated. Replaces the in-process EF lookup that the
    /// monolith used. Returns null when the selection is invalid or Catalog is unreachable.
    /// </summary>
    public class CatalogClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<CatalogClient> _logger;

        public CatalogClient(HttpClient http, ILogger<CatalogClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<CategorySelectionDto?> ValidateSelectionAsync(int categoryId, int subCategoryId)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"/internal/validate?categoryId={categoryId}&subCategoryId={subCategoryId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<CategorySelectionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Catalog service unreachable while validating selection.");
                throw new InvalidOperationException("Catalog service is unavailable. Please try again later.");
            }
        }
    }
}
