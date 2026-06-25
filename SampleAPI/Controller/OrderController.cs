using Microsoft.AspNetCore.Mvc;
using SampleAPI.Database.Models;
using SampleAPI.Database.Services;

namespace SampleAPI.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(ICustomerOrderService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await service.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await service.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreatedDto order)
        {
            try
            {
                var createdOrder = await service.CreateOrder(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
