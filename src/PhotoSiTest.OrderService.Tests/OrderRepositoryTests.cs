using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.OrderService.Core.Models;
using PhotoSiTest.OrderService.Data;
using PhotoSiTest.OrderService.Data.Repositories;

namespace PhotoSiTest.OrderService.Tests
{
    public class OrderRepositoryTests
    {
        private readonly OrderRepository _orderRepository;
        private readonly DbContextOptions<OrderContext> _options;

        public OrderRepositoryTests()
        {
            // Configura un database in memoria per i test
            _options = new DbContextOptionsBuilder<OrderContext>()
                      .UseInMemoryDatabase(databaseName: "OrderDatabase")
                      .Options;

            // Inizializza il repository con il contesto in memoria
            var context = new OrderContext(_options);
            _orderRepository = new OrderRepository(context);
        }

        [Fact]
        public async Task AddOrder_ShouldAddOrderToDatabase()
        {
            // Arrange
            var order = new Order
            {
                UserId = 123,
                OrderDate = DateTime.Now,
                TotalAmount = 100m,
                ProductIds = new List<int> { 1, 2 },
                Status = OrderStatus.Pending,
                AddressId = 1
            };

            // Act
            await _orderRepository.AddAsync(order);

            // Assert
            using (var context = new OrderContext(_options))
            {
                var savedOrder = await context.Orders.FindAsync(order.Id);

                // Verifica che il savedOrder non sia null
                savedOrder.Should().NotBeNull();

                // Confronta i valori delle proprietà
                savedOrder.UserId.Should().Be(order.UserId);
                savedOrder.TotalAmount.Should().Be(order.TotalAmount);
            }
        }
    }

}
