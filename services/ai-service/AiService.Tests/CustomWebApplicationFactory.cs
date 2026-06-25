using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AiService.Data;
using AiService.DTOs;
using AiService.Services;

namespace AiService.Tests
{
    /// <summary>
    /// Configurable catalog stub so prompt tests don't need the catalog-service running.
    /// By default returns a valid selection; set Selection = null to simulate an invalid pick.
    /// </summary>
    public class FakeCatalogClient : ICatalogClient
    {
        public CategorySelectionDto? Selection { get; set; } = new()
        {
            CategoryId = 1,
            CategoryName = "Programming",
            SubCategoryId = 1,
            SubCategoryName = "C#"
        };

        public Task<CategorySelectionDto?> ValidateSelectionAsync(int categoryId, int subCategoryId)
            => Task.FromResult(Selection);
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public FakeCatalogClient Catalog { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(ICatalogClient));
                services.AddSingleton<ICatalogClient>(Catalog);

                // Never call the real OpenAI API in tests.
                services.RemoveAll(typeof(ILessonGenerator));
                services.AddSingleton<ILessonGenerator, FakeLessonGenerator>();
            });
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PromptsDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }
    }
}
