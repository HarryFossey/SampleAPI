using SampleAPI.Database.Context.Entities;

namespace SampleAPI.Database.Models
{
    public class OrderCreatedDto
    {
        public required string CustomerName { get; set; }
        public decimal OrderValue { get; set; }
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public Order ToEntity()
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = CustomerName,
                OrderValue = OrderValue,
                OrderDate = OrderDate
            };
        }
    }
}
