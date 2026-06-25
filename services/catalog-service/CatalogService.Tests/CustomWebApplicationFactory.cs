using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CatalogService.Data;
using CatalogService.Services;

namespace CatalogService.Tests
{
    /// <summary>
    /// Configurable fake so delete-guard tests don't need the AI service running.
    /// </summary>
    public class FakePromptUsageClient : IPromptUsageClient
    {
        public bool CategoryInUse { get; set; }
        public bool SubCategoryInUse { get; set; }
        public Task<bool> CategoryInUseAsync(int categoryId) => Task.FromResult(CategoryInUse);
        public Task<bool> SubCategoryInUseAsync(int subCategoryId) => Task.FromResult(SubCategoryInUse);
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public FakePromptUsageClient Usage { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IPromptUsageClient));
                services.AddSingleton<IPromptUsageClient>(Usage);
            });
        }

        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            await CatalogSeeder.SeedAsync(db);
        }
    }
}
