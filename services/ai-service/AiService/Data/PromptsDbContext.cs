using Microsoft.EntityFrameworkCore;
using AiService.Models;

namespace AiService.Data
{
    public class PromptsDbContext : DbContext
    {
        public PromptsDbContext(DbContextOptions<PromptsDbContext> options) : base(options) { }

        public DbSet<Prompt> Prompts { get; set; }
    }
}
