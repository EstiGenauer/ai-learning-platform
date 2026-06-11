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
        }
    }
}
