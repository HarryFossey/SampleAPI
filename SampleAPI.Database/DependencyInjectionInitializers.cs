using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleAPI.Database.Context;
using SampleAPI.Database.Models.Config;
using SampleAPI.Database.Services;

namespace SampleAPI.Database
{
    public static class DependencyInjectionInitializers
    {
        public static void AddDatabases(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<CustomerOrderDbContext>(options =>
            {
                options.UseSqlServer("name=ConnectionStrings:CustomerOrder");
            });
        }

        public static void SetOrderSettings(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<OrderSettings>(configuration.GetSection("OrderSettings"));
        }

        public static void AddCustomServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICustomerOrderService, CustomerOrderService>();
        }
    }
}
