using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSiTest.OrderService.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<PhotoSi.OrderService.Services.OrderService>> _mockLogger;
        private readonly PhotoSi.OrderService.Services.OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<PhotoSi.OrderService.Services.OrderService>>();
            _orderService = new PhotoSi.OrderService.Services.OrderService(_mockOrderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithAddressAsync_ShouldReturnTrue_WhenPendingOrProcessingOrdersExist()
        {
            // Arrange
            int addressId = 1;

            var orders = new List<Order>
            {
                new Order { AddressId = addressId, Status = OrderStatus.Pending },
                new Order { AddressId = addressId, Status = OrderStatus.Completed }
            };

            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(orders.AsQueryable().BuildMock());


            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithAddressAsync(addressId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithAddressAsync_ShouldReturnFalse_WhenNoPendingOrProcessingOrdersExist()
        {
            // Arrange
            int addressId = 1;
            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>
                {
                    new Order { AddressId = addressId, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMock());

            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithAddressAsync(addressId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithProductAsync_ShouldReturnTrue_WhenPendingOrProcessingOrdersContainProduct()
        {
            // Arrange
            int productId = 1;
            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>
                {
                    new Order { ProductIds = new List<int> { productId }, Status = OrderStatus.Pending },
                    new Order { ProductIds = new List<int> { productId }, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMock());

            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithProductAsync(productId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithProductAsync_ShouldReturnFalse_WhenNoPendingOrProcessingOrdersContainProduct()
        {
            // Arrange
            int productId = 1;
            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>
                {
                    new Order { ProductIds = new List<int> { 2 }, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMock());

            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithProductAsync(productId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithUserAsync_ShouldReturnTrue_WhenPendingOrProcessingOrdersExistForUser()
        {
            // Arrange
            int userId = 1;
            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>
                {
                    new Order { UserId = userId, Status = OrderStatus.Pending },
                    new Order { UserId = userId, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMock());

            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithUserAsync(userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPendingOrProcessingOrdersWithUserAsync_ShouldReturnFalse_WhenNoPendingOrProcessingOrdersExistForUser()
        {
            // Arrange
            int userId = 1;
            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>
                {
                    new Order { UserId = userId, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMock());

            // Act
            var result = await _orderService.HasPendingOrProcessingOrdersWithUserAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsDuplicateOrderAsync_ShouldReturnTrue_WhenOrderExists()
        {
            // Arrange
            var orderDto = new OrderEditDto
            {
                UserId = 1,
                AddressId = 2,
                OrderDate = new DateTime(2024, 11, 24)
            };

            var existingOrder = new Order
            {
                UserId = 1,
                AddressId = 2,
                OrderDate = new DateTime(2024, 11, 24) // L'ordine esiste con la stessa data
            };

            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order> { existingOrder }.AsQueryable().BuildMock()); // Simula un ordine esistente

            // Act
            var result = await _orderService.IsDuplicateOrderAsync(orderDto);

            // Assert
            result.Should().BeTrue(); // Dovrebbe restituire true, poiché l'ordine esiste già
        }

        [Fact]
        public async Task IsDuplicateOrderAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderDto = new OrderEditDto
            {
                UserId = 1,
                AddressId = 2,
                OrderDate = new DateTime(2024, 11, 24)
            };

            _mockOrderRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Order>().AsQueryable().BuildMock()); // Simula nessun ordine esistente

            // Act
            var result = await _orderService.IsDuplicateOrderAsync(orderDto);

            // Assert
            result.Should().BeFalse(); // Dovrebbe restituire false, poiché l'ordine non esiste
        }
    }
}
