using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AiService.DTOs;

namespace AiService.Tests
{
    public class PromptEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public PromptEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private HttpClient ClientFor(int userId, string name)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestJwt.Create(userId, name));
            return client;
        }

        private static CreatePromptRequest ValidRequest() => new()
        {
            CategoryId = 1,
            SubCategoryId = 1,
            PromptText = "Teach me about async/await"
        };

        [Fact]
        public async Task CreatePrompt_WithoutToken_ReturnsUnauthorized()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/Prompts", ValidRequest());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreatePrompt_WithValidSelection_GeneratesAndPersistsLesson()
        {
            await _factory.ResetDatabaseAsync();
            _factory.Catalog.Selection = new CategorySelectionDto
            {
                CategoryId = 1,
                CategoryName = "Programming",
                SubCategoryId = 1,
                SubCategoryName = "C#"
            };
            var client = ClientFor(7, "Alice");

            var response = await client.PostAsJsonAsync("/api/Prompts", ValidRequest());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.Content.ReadFromJsonAsync<PromptDto>();
            Assert.NotNull(dto);
            Assert.Equal(7, dto!.UserId);
            Assert.Equal("C#", dto.SubCategoryName);
            Assert.False(string.IsNullOrWhiteSpace(dto.Response));
        }

        [Fact]
        public async Task CreatePrompt_WithInvalidSelection_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();
            _factory.Catalog.Selection = null; // catalog rejects the selection
            var client = ClientFor(7, "Alice");

            var response = await client.PostAsJsonAsync("/api/Prompts", ValidRequest());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreatePrompt_WithTooShortPromptText_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();
            var client = ClientFor(7, "Alice");

            var response = await client.PostAsJsonAsync("/api/Prompts", new CreatePromptRequest
            {
                CategoryId = 1,
                SubCategoryId = 1,
                PromptText = "ab"
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task History_ReturnsOnlyCurrentUsersPrompts()
        {
            await _factory.ResetDatabaseAsync();
            _factory.Catalog.Selection = new CategorySelectionDto
            {
                CategoryId = 1, CategoryName = "Programming", SubCategoryId = 1, SubCategoryName = "C#"
            };

            var alice = ClientFor(7, "Alice");
            var bob = ClientFor(8, "Bob");
            await alice.PostAsJsonAsync("/api/Prompts", ValidRequest());
            await alice.PostAsJsonAsync("/api/Prompts", ValidRequest());
            await bob.PostAsJsonAsync("/api/Prompts", ValidRequest());

            var aliceHistory = await alice.GetFromJsonAsync<List<PromptDto>>("/api/Prompts/history");
            Assert.Equal(2, aliceHistory!.Count);
            Assert.All(aliceHistory, p => Assert.Equal(7, p.UserId));
        }

        [Fact]
        public async Task InternalPromptCounts_ReturnsCountsPerUser()
        {
            await _factory.ResetDatabaseAsync();
            _factory.Catalog.Selection = new CategorySelectionDto
            {
                CategoryId = 1, CategoryName = "Programming", SubCategoryId = 1, SubCategoryName = "C#"
            };

            var alice = ClientFor(7, "Alice");
            await alice.PostAsJsonAsync("/api/Prompts", ValidRequest());
            await alice.PostAsJsonAsync("/api/Prompts", ValidRequest());

            var client = _factory.CreateClient();
            var counts = await client.GetFromJsonAsync<Dictionary<int, int>>("/internal/prompt-counts");
            Assert.Equal(2, counts![7]);
        }

        [Fact]
        public async Task InternalPromptUsage_ReturnsTrueWhenCategoryReferenced()
        {
            await _factory.ResetDatabaseAsync();
            _factory.Catalog.Selection = new CategorySelectionDto
            {
                CategoryId = 42, CategoryName = "Programming", SubCategoryId = 1, SubCategoryName = "C#"
            };

            var alice = ClientFor(7, "Alice");
            await alice.PostAsJsonAsync("/api/Prompts", new CreatePromptRequest
            {
                CategoryId = 42, SubCategoryId = 1, PromptText = "Teach me something"
            });

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/internal/prompt-usage?categoryId=42");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            Assert.True(json.RootElement.GetProperty("inUse").GetBoolean());
        }
    }
}
