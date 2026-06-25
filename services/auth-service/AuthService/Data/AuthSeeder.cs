using Microsoft.EntityFrameworkCore;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Data
{
    public static class AuthSeeder
    {
        public static async Task SeedAsync(AuthDbContext context, PasswordService passwordService)
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
