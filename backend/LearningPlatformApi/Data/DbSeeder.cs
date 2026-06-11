using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Models;
using LearningPlatformApi.Services;

namespace LearningPlatformApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, PasswordService passwordService)
        {
            if (!await context.Users.AnyAsync(u => u.IsAdmin))
            {
                context.Users.Add(new User
                {
                    Name = "Admin",
                    Email = "admin@admin.com",
                    Phone = "0500000000",
                    PasswordHash = passwordService.Hash("Admin123!"),
                    IsAdmin = true
                });
                await context.SaveChangesAsync();
            }

            await SeedCategoriesAsync(context);
        }

        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            foreach (var (categoryName, subNames) in CategoriesSeedData.Categories)
            {
                var category = await context.Categories
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.Name == categoryName);

                if (category == null)
                {
                    category = new Category { Name = categoryName };
                    context.Categories.Add(category);
                    await context.SaveChangesAsync();
                }

                foreach (var subName in subNames)
                {
                    var exists = await context.SubCategories.AnyAsync(s =>
                        s.CategoryId == category.Id && s.Name == subName);

                    if (!exists)
                    {
                        context.SubCategories.Add(new SubCategory
                        {
                            Name = subName,
                            CategoryId = category.Id
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"Categories seeded: {CategoriesSeedData.Categories.Count} categories loaded.");
        }
    }
}
