using SampleAPI.Database.Models;

namespace SampleAPI.Database.Tests.Helpers
{
    public static class TestDataFactory
    {
        public static OrderCreatedDto CreateOrderCreatedDto(
            string? customerName = null,
            decimal orderValue = 250.00m,
            DateOnly? orderDate = null)
        {
            return new OrderCreatedDto
            {
                CustomerName = customerName ?? "TestName",
                OrderValue = orderValue,
                OrderDate = orderDate ?? DateOnly.FromDateTime(DateTime.UtcNow.Date)
            };
        }

        public static OrderDto CreateOrderDto(
            Guid? id = null,
            string? customerName = null,
            decimal orderValue = 250.00m,
            DateOnly? orderDate = null)
        {
            return new OrderDto
            {
                Id = id ?? Guid.NewGuid(),
                CustomerName = customerName ?? "TestName",
                OrderValue = orderValue,
                OrderDate = orderDate ?? DateOnly.FromDateTime(DateTime.UtcNow.Date)
            };
        }
    }
}
