using Microsoft.EntityFrameworkCore;
using SampleAPI.Database.Context;

namespace SampleAPI.Database.Tests.Fixtures
{
    public sealed class CustomerOrderFixture : IDisposable
    {
        public CustomerOrderFixture()
        {
            var options = new DbContextOptionsBuilder<CustomerOrderDbContext>()
                .UseInMemoryDatabase(databaseName: $"DBTestInMemory:{Guid.NewGuid()}")
                .Options;

            DbContext = new CustomerOrderDbContext(options);
        }
        public CustomerOrderDbContext DbContext { get; }
        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
