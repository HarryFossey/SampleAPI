using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleAPI.Controller;
using SampleAPI.Database.Models;
using SampleAPI.Database.Services;

namespace SampleAPI.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<ICustomerOrderService> _service = new();
        private readonly OrderControllerTests _sut;

        [Fact]
        public async Task GetAllOrders_ReturnsOkResult_WithOrders()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            var orders = new List<OrderDto>
            {
                GenerateOrderDto()
            };
            mockService.Setup(s => s.GetAllOrders()).ReturnsAsync(orders);

            var controller = new OrderController(mockService.Object);

            // Act
            var result = await controller.GetAllOrders();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IList<OrderDto>>(ok.Value);
            Assert.Single(returned);
            Assert.Equal("Acme Ltd", returned[0].CustomerName);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            var id = Guid.NewGuid();
            mockService.Setup(s => s.GetOrderById(id)).ReturnsAsync((OrderDto?)null);

            var controller = new OrderController(mockService.Object);

            // Act
            var result = await controller.GetOrderById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_WhenFound()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            var order = GenerateOrderDto();
            var id = order.Id;
            mockService.Setup(s => s.GetOrderById(id)).ReturnsAsync(order);

            var controller = new OrderController(mockService.Object);

            // Act
            var result = await controller.GetOrderById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<OrderDto>(ok.Value);
            Assert.Equal(id, returned.Id);
            Assert.Equal("Acme Ltd", returned.CustomerName);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            var created = GenerateOrderDto();
            mockService.Setup(s => s.CreateOrder(It.IsAny<OrderCreatedDto>())).ReturnsAsync(created);

            var controller = new OrderController(mockService.Object);
            var newOrder = new OrderCreatedDto
            {
                CustomerName = "Acme Ltd",
                OrderValue = 250.00m,
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            // Act
            var result = await controller.CreateOrder(newOrder);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<OrderDto>(createdResult.Value);
            Assert.Equal(created.Id, returned.Id);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_OnArgumentException()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            mockService
                .Setup(s => s.CreateOrder(It.IsAny<OrderCreatedDto>()))
                .ThrowsAsync(new ArgumentException("Invalid order"));

            var controller = new OrderController(mockService.Object);
            var newOrder = GenerateOrderCreatedDto();

            // Act
            var result = await controller.CreateOrder(newOrder);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid order", bad.Value);
        }

        [Fact]
        public async Task CreateOrder_ReturnsConflict_OnInvalidOperationException()
        {
            // Arrange
            var mockService = new Mock<ICustomerOrderService>();
            mockService
                .Setup(s => s.CreateOrder(It.IsAny<OrderCreatedDto>()))
                .ThrowsAsync(new InvalidOperationException("Duplicate"));

            var controller = new OrderController(mockService.Object);
            var newOrder = GenerateOrderCreatedDto();

            // Act
            var result = await controller.CreateOrder(newOrder);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Duplicate", conflict.Value);
        }

        private static OrderDto GenerateOrderDto()
        {
            return new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerName = "Acme Ltd",
                OrderValue = 250.00m,
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }

        private static OrderCreatedDto GenerateOrderCreatedDto()
        {
            return new OrderCreatedDto
            {
                CustomerName = "Acme Ltd",
                OrderValue = 250.00m,
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }

    }
}
