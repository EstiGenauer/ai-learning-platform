using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthService.DTOs;

namespace AuthService.Tests
{
    public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_ThenLogin_ReturnsJwtToken()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();
            var email = $"user-{Guid.NewGuid():N}@test.com";

            var register = await client.PostAsJsonAsync("/api/Auth/register", new RegisterRequest
            {
                Name = "Test User",
                Email = email,
                Password = "Password123!",
                Phone = "0501234567"
            });
            Assert.Equal(HttpStatusCode.OK, register.StatusCode);

            var login = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = email,
                Password = "Password123!"
            });
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);

            var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth!.Token));
            Assert.Equal(email, auth.User.Email);
            Assert.False(auth.User.IsAdmin);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();
            var email = $"dupe-{Guid.NewGuid():N}@test.com";
            var body = new RegisterRequest { Name = "Dupe", Email = email, Password = "Password123!" };

            Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/Auth/register", body)).StatusCode);
            var second = await client.PostAsJsonAsync("/api/Auth/register", body);
            Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
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

        [Fact]
        public async Task AdminUsers_WithoutToken_ReturnsUnauthorized()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Admin/users");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AdminUsers_WithAdminToken_ReturnsUsersWithPromptCounts()
        {
            await _factory.ResetDatabaseAsync();
            var client = _factory.CreateClient();

            var login = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "admin@admin.com",
                Password = "Admin123!"
            });
            var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

            // The admin user gets id 1 from the in-memory store; simulate 5 prompts via the fake stats client.
            _factory.Stats.Counts = new Dictionary<int, int> { [auth.User.Id] = 5 };

            var response = await client.GetAsync("/api/Admin/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var adminEntry = json.RootElement.EnumerateArray()
                .First(u => u.GetProperty("email").GetString() == "admin@admin.com");
            Assert.Equal(5, adminEntry.GetProperty("promptCount").GetInt32());
        }
    }
}
