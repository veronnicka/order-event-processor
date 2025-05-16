using Microsoft.EntityFrameworkCore;
using OrderEventProcessor.Models;

namespace OrderEventProcessor.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<OrderEvent> OrderEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=orderdb;Username=user;Password=password");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderEvent>().HasKey(o => o.Id);
        }
    }
}