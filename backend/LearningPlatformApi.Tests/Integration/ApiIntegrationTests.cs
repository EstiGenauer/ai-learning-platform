using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LearningPlatformApi.DTOs;

namespace LearningPlatformApi.Tests
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_ThenLogin_ReturnsJwtToken()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();
            var email = $"user-{Guid.NewGuid():N}@test.com";

            var registerResponse = await client.PostAsJsonAsync("/api/Auth/register", new RegisterRequest
            {
                Name = "Test User",
                Email = email,
                Password = "Password123!",
                Phone = "0501234567"
            });

            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123!"
            });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth!.Token));
            Assert.Equal(email, auth.User.Email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "admin@admin.com",
                Password = "wrong-password"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    public class CategoriesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CategoriesIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetCategories_ReturnsSeededCategoriesWithoutAuth()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Categories");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            Assert.True(json.RootElement.GetArrayLength() >= 40);
        }
    }

    public class PromptsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public PromptsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreatePrompt_WithValidAuth_ReturnsGeneratedLesson()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "admin@admin.com",
                Password = "Admin123!"
            });
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth!.Token);

            var categoriesResponse = await client.GetAsync("/api/Categories");
            using var categoriesJson = await JsonDocument.ParseAsync(await categoriesResponse.Content.ReadAsStreamAsync());
            var firstCategory = categoriesJson.RootElement[0];
            var categoryId = firstCategory.GetProperty("id").GetInt32();
            var subCategoryId = firstCategory.GetProperty("subCategories")[0].GetProperty("id").GetInt32();

            var promptResponse = await client.PostAsJsonAsync("/api/Prompts", new
            {
                categoryId,
                subCategoryId,
                promptText = "Explain this topic briefly"
            });

            Assert.Equal(HttpStatusCode.OK, promptResponse.StatusCode);

            using var promptJson = await JsonDocument.ParseAsync(await promptResponse.Content.ReadAsStreamAsync());
            var responseText = promptJson.RootElement.GetProperty("response").GetString();
            Assert.Contains("Test lesson", responseText);
        }

        [Fact]
        public async Task CreatePrompt_WithoutAuth_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/Prompts", new
            {
                categoryId = 1,
                subCategoryId = 1,
                promptText = "test"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    public class HealthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        [Fact]
        public async Task Health_ReturnsOk()
        {
            var client = new CustomWebApplicationFactory().CreateClient();
            var response = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
