using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSi.OrderService.Core.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}