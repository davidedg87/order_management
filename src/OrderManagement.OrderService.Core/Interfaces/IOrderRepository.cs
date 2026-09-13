using OrderManagement.Common.Interfaces;
using OrderManagement.OrderService.Core.Models;

namespace OrderManagement.OrderService.Core.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}