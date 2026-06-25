using Microsoft.EntityFrameworkCore;
using CatalogService.Models;

namespace CatalogService.Data
{
    public static class CatalogSeeder
    {
        public static async Task SeedAsync(CatalogDbContext context)
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
            Console.WriteLine($"catalog-service: {CategoriesSeedData.Categories.Count} categories seeded.");
        }
    }
}
