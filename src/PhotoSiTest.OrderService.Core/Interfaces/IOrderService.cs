
using PhotoSi.OrderService.Core.Dtos;
using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSi.OrderService.Core.Interfaces
{
    public interface IOrderService : IBaseService<Order, OrderDto, OrderEditDto>
    {
        Task<bool> HasPendingOrProcessingOrdersWithAddressAsync(int addressId);

        Task<bool> HasPendingOrProcessingOrdersWithProductAsync(int productId);

        Task<bool> HasPendingOrProcessingOrdersWithUserAsync(int userId);

        Task<bool> IsDuplicateOrderAsync(OrderEditDto orderDto);

    }
}