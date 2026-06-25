using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SampleAPI.Database.Models.Config;
using SampleAPI.Database.Services;
using SampleAPI.Database.Tests.Fixtures;
using SampleAPI.Database.Tests.Helpers;

namespace SampleAPI.Database.Tests.Services
{
    public sealed class CustomerOrderServiceTests : IDisposable
    {
        private readonly CustomerOrderFixture _fixture;
        private readonly CustomerOrderService _sut;
        private readonly Mock<ILogger<CustomerOrderService>> _loggerMock;
        private readonly IOptions<OrderSettings> _options;

        private const decimal MaxOrderValue = 10000m;

        public CustomerOrderServiceTests()
        {
            _fixture = new CustomerOrderFixture();
            _loggerMock = new Mock<ILogger<CustomerOrderService>>();
            _options = Options.Create(new OrderSettings { MaxOrderValue = MaxOrderValue });
            _sut = new CustomerOrderService(_fixture.DbContext, _loggerMock.Object, _options);
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        [Fact]
        public async Task CreateOrder_Succeeds_WhenValid()
        {

            // Arrange
            var dto = TestDataFactory.CreateOrderCreatedDto(customerName: "John Doe", orderValue: 500.00m);

            // Act
            var result = await _sut.CreateOrder(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CustomerName, result.CustomerName);
            Assert.Equal(dto.OrderValue, result.OrderValue);
            Assert.Equal(dto.OrderDate, result.OrderDate);
        }

        [Fact]
        public async Task CreateOrder_ThrowsArgumentException_WhenValueExceedsMax()
        {
            // Arrange
            var dto = TestDataFactory.CreateOrderCreatedDto(customerName: "John Doe", orderValue: MaxOrderValue + 1); // Exceeds max of 10000

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateOrder(dto));
        }

        [Fact]
        public async Task CreateOrder_ThrowsInvalidOperationException_OnDuplicate()
        {
            // Arrange
            var dto = TestDataFactory.CreateOrderCreatedDto(customerName: "John Doe", orderValue: 500.00m);

            // Act
            await _sut.CreateOrder(dto);

            // Duplicate attempt should throw
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateOrder(dto));
        }

        [Fact]
        public async Task GetAllOrders_ReturnsEmptyList_WhenNoOrders()
        {
            // Act
            var result = await _sut.GetAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsAllOrders_WhenMultipleExist()
        {
            // Arrange
            var dto1 = TestDataFactory.CreateOrderCreatedDto(customerName: "Customer A", orderValue: 100.00m);
            var dto2 = TestDataFactory.CreateOrderCreatedDto(customerName: "Customer B", orderValue: 200.00m);
            var dto3 = TestDataFactory.CreateOrderCreatedDto(customerName: "Customer C", orderValue: 300.00m);

            var created1 = await _sut.CreateOrder(dto1);
            var created2 = await _sut.CreateOrder(dto2);
            var created3 = await _sut.CreateOrder(dto3);

            // Act
            var result = await _sut.GetAllOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, o => o.Id == created1.Id);
            Assert.Contains(result, o => o.Id == created2.Id);
            Assert.Contains(result, o => o.Id == created3.Id);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Act
            var result = await _sut.GetOrderById(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenExists()
        {
            // Arrange
            var dto = TestDataFactory.CreateOrderCreatedDto(
                customerName: "John Doe",
                orderValue: 1000.00m);

            var created = await _sut.CreateOrder(dto);

            // Act
            var result = await _sut.GetOrderById(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result!.Id);
            Assert.Equal(dto.CustomerName, result.CustomerName);
            Assert.Equal(dto.OrderValue, result.OrderValue);
            Assert.Equal(dto.OrderDate, result.OrderDate);
        }

        [Fact]
        public async Task GetOrderById_ReturnsCorrectOrder_WhenMultipleExist()
        {
            // Arrange
            var dto1 = TestDataFactory.CreateOrderCreatedDto(customerName: "First", orderValue: 100.00m);
            var dto2 = TestDataFactory.CreateOrderCreatedDto(customerName: "Second", orderValue: 200.00m);
            var dto3 = TestDataFactory.CreateOrderCreatedDto(customerName: "Third", orderValue: 300.00m);

            await _sut.CreateOrder(dto1);
            var created2 = await _sut.CreateOrder(dto2);
            await _sut.CreateOrder(dto3);

            // Act
            var result = await _sut.GetOrderById(created2.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created2.Id, result!.Id);
            Assert.Equal("Second", result.CustomerName);
            Assert.Equal(200.00m, result.OrderValue);
        }
    }
}
