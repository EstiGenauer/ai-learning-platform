using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AuthService.Data;
using AuthService.Services;

namespace AuthService.Tests
{
    /// <summary>
    /// Fake stats client so admin-user tests do not need the AI service running.
    /// </summary>
    public class FakePromptStatsClient : IPromptStatsClient
    {
        public Dictionary<int, int> Counts { get; set; } = new();
        public Task<Dictionary<int, int>> GetPromptCountsAsync() => Task.FromResult(Counts);
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public FakePromptStatsClient Stats { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IPromptStatsClient));
                services.AddSingleton<IPromptStatsClient>(Stats);
            });
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            await AuthSeeder.SeedAsync(db, passwordService);
        }
    }
}
