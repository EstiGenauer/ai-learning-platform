using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Data;
using LearningPlatformApi.Services;

namespace LearningPlatformApi.Tests.Data
{
    public class DbSeederTests
    {
        [Fact]
        public async Task SeedCategoriesAsync_AddsMissingCategoriesAndSubCategories()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            await DbSeeder.SeedCategoriesAsync(context);

            var categoryCount = await context.Categories.CountAsync();
            var subCount = await context.SubCategories.CountAsync();

            Assert.True(categoryCount >= CategoriesSeedData.Categories.Count);
            Assert.True(subCount >= 200);
        }

        [Fact]
        public async Task SeedAsync_CreatesAdminUserWhenMissing()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var passwordService = new PasswordService();
            await DbSeeder.SeedAsync(context, passwordService);

            var admin = await context.Users.SingleAsync(u => u.IsAdmin);
            Assert.Equal("admin@admin.com", admin.Email);
            Assert.True(passwordService.Verify("Admin123!", admin.PasswordHash));
        }
    }
}
