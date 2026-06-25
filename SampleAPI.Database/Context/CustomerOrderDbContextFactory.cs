using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SampleAPI.Database.Context
{
    public class CustomerOrderDbContextFactory : IDesignTimeDbContextFactory<CustomerOrderDbContext>
    {
        public CustomerOrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomerOrderDbContext>();
            optionsBuilder.UseSqlServer(string.Join(" ", args));
            return new CustomerOrderDbContext(optionsBuilder.Options);
        }
    }
}