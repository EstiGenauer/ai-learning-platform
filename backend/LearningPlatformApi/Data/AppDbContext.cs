using Microsoft.EntityFrameworkCore;
using LearningPlatformApi.Models;

namespace LearningPlatformApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Prompt> Prompts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<SubCategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prompt>()
                .HasOne(p => p.User)
                .WithMany(u => u.Prompts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prompt>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prompt>()
                .HasOne(p => p.SubCategory)
                .WithMany()
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming" },
                new Category { Id = 2, Name = "Data Science" }
            );

            modelBuilder.Entity<SubCategory>().HasData(
                new SubCategory { Id = 1, Name = "C#", CategoryId = 1 },
                new SubCategory { Id = 2, Name = "JavaScript", CategoryId = 1 },
                new SubCategory { Id = 3, Name = "Python", CategoryId = 1 },
                new SubCategory { Id = 4, Name = "Machine Learning", CategoryId = 2 },
                new SubCategory { Id = 5, Name = "Statistics", CategoryId = 2 },
                new SubCategory { Id = 6, Name = "Data Visualization", CategoryId = 2 }
            );
        }
    }
}
