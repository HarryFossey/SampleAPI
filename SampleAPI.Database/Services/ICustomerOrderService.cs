using SampleAPI.Database.Models;

namespace SampleAPI.Database.Services
{
    public interface ICustomerOrderService
    {
        Task<IList<OrderDto>> GetAllOrders();
        Task<OrderDto?> GetOrderById(Guid id);
        Task<OrderDto> CreateOrder(OrderCreatedDto order);
    }
}
