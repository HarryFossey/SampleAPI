using Microsoft.EntityFrameworkCore;
using SampleAPI.Database.Context.Entities;

namespace SampleAPI.Database.Context
{
    public class CustomerOrderDbContext(DbContextOptions<CustomerOrderDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasIndex(x => x.OrderValue);
            modelBuilder.Entity<Order>()
                .HasIndex(x => new { x.CustomerName, x.OrderDate, x.OrderValue })
                .IsUnique();
        }
    }
}
