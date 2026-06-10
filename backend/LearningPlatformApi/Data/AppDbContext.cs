using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Models;

namespace LearningPlatformApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // הגדרת הטבלאות
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<PromptHistory> PromptHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // הגדרת קשרים בין טבלאות
            modelBuilder.Entity<SubCategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(s => s.CategoryId);

            // הוספת נתונים ראשוניים (Seeding)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming" },
                new Category { Id = 2, Name = "Data Science" }
            );

            modelBuilder.Entity<PromptHistory>().HasData(
                new PromptHistory { Id = 1, Question = "What is C#?", Answer = "C# is a modern, object-oriented programming language." },
                new PromptHistory { Id = 2, Question = "What is Python?", Answer = "Python is a high-level, interpreted programming language." }
            );
        }
    }
}