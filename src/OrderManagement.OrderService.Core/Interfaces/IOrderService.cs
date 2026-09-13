
using OrderManagement.OrderService.Core.Dtos;
using OrderManagement.Common.Interfaces;
using OrderManagement.OrderService.Core.Models;

namespace OrderManagement.OrderService.Core.Interfaces
{
    public interface IOrderService : IBaseService<Order, OrderDto, OrderEditDto>
    {
        Task<bool> HasPendingOrProcessingOrdersWithAddressAsync(int addressId);

        Task<bool> HasPendingOrProcessingOrdersWithProductAsync(int productId);

        Task<bool> HasPendingOrProcessingOrdersWithUserAsync(int userId);

        Task<bool> IsDuplicateOrderAsync(OrderEditDto orderDto);

    }
}