using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSiTest.Common.Services;
using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSi.OrderService.Services
{
    public class OrderService : BaseService<Order, OrderDto, OrderEditDto>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger) : base(orderRepository, logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<bool> HasPendingOrProcessingOrdersWithAddressAsync(int addressId)
        {
            _logger.LogTrace("Checking if there are any Pending or Processing orders with Address ID: {AddressId}", addressId);

            var result = await _orderRepository.Query()
                                                .AnyAsync(order => order.AddressId == addressId &&
                                                                   (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing));

            if (result)
            {
                _logger.LogTrace("There are Pending or Processing orders with Address ID: {AddressId}", addressId);
            }
            else
            {
                _logger.LogTrace("No Pending or Processing orders found for Address ID: {AddressId}", addressId);
            }

            return result;
        }

        public async Task<bool> HasPendingOrProcessingOrdersWithProductAsync(int productId)
        {
            _logger.LogTrace("Checking if there are any Pending or Processing orders with Product ID: {ProductId}", productId);

            var result = await _orderRepository.Query()
                                                .AnyAsync(order => order.ProductIds.Contains(productId) &&
                                                                   (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing));

            if (result)
            {
                _logger.LogTrace("There are Pending or Processing orders with Product ID: {ProductId}", productId);
            }
            else
            {
                _logger.LogTrace("No Pending or Processing orders found for Product ID: {ProductId}", productId);
            }

            return result;
        }

        public async Task<bool> HasPendingOrProcessingOrdersWithUserAsync(int userId)
        {
            _logger.LogTrace("Checking if there are any Pending or Processing orders with User ID: {UserId}", userId);

            var result = await _orderRepository.Query()
                                                .AnyAsync(order => order.UserId == userId &&
                                                                   (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing));

            if (result)
            {
                _logger.LogTrace("There are Pending or Processing orders with User ID: {UserId}", userId);
            }
            else
            {
                _logger.LogTrace("No Pending or Processing orders found for User ID: {UserId}", userId);
            }

            return result;
        }

        public async Task<bool> IsDuplicateOrderAsync(OrderEditDto orderDto)
        {
            _logger.LogTrace("Checking if order exists for UserId: {UserId}, AddressId: {AddressId}, OrderDate: {OrderDate}",
                orderDto.UserId, orderDto.AddressId, orderDto.OrderDate);

            // Verifica se esiste già un ordine con gli stessi valori
            var existingOrder = await _orderRepository.Query()
                .FirstOrDefaultAsync(o => o.UserId == orderDto.UserId &&
                                          o.AddressId == orderDto.AddressId &&
                                          o.OrderDate.Date == orderDto.OrderDate.Date); // Aggiunta la condizione .Date per comparare solo la data, ignorando l'orario

            return existingOrder != null; // Restituisce true se esiste un duplicato
        }

    }
}
