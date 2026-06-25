using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleAPI.Database.Context;
using SampleAPI.Database.Context.Entities;
using SampleAPI.Database.Models;
using SampleAPI.Database.Models.Config;

namespace SampleAPI.Database.Services
{
    public class CustomerOrderService(CustomerOrderDbContext context, ILogger<CustomerOrderService> logger, IOptions<OrderSettings> orderSettings) : ICustomerOrderService
    {
        public async Task<IList<OrderDto>> GetAllOrders()
        {
            return await context.Orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderValue = order.OrderValue
            }).ToListAsync();
        }

        public async Task<OrderDto?> GetOrderById(Guid id)
        {
            var order = await context.Orders.FindAsync(id);
            if (order == null)
            {
                logger.LogInformation("Order with Id: {OrderId} not found.", id);
                return null;
            }
            return OrdersToOrderDto(order);
        }

        public async Task<OrderDto> CreateOrder(OrderCreatedDto order)
        {
            if (order.OrderValue > orderSettings.Value.MaxOrderValue)
            {
                logger.LogWarning(
                    "Order value {OrderValue} exceeds maximum allowed value {MaxOrderValue}",
                    order.OrderValue,
                    orderSettings.Value.MaxOrderValue);
                throw new ArgumentException($"Order value exceeds the maximum allowed value of {orderSettings.Value.MaxOrderValue}.");
            }

            var checkOrders = await context.Orders.AnyAsync(o =>
                o.CustomerName == order.CustomerName &&
                o.OrderDate == order.OrderDate &&
                o.OrderValue == order.OrderValue);

            if (checkOrders)
            {
                logger.LogWarning(
                    "Duplicate order detected. CustomerName={CustomerName}, OrderDate={OrderDate}, OrderValue={OrderValue}",
                    order.CustomerName, order.OrderDate, order.OrderValue);

                throw new InvalidOperationException("Order already exists with values: " +
                    $"CustomerName={order.CustomerName}, OrderDate={order.OrderDate}, OrderValue={order.OrderValue}");
            }

            var entity = order.ToEntity();
            context.Orders.Add(entity);
            await context.SaveChangesAsync();

            logger.LogInformation("Order created with Id: {OrderId}", entity.Id);

            return new OrderDto
            {
                Id = entity.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderValue = order.OrderValue
            };
        }

        private static OrderDto OrdersToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                OrderValue = order.OrderValue
            };
        }
    }
}
