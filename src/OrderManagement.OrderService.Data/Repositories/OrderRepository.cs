using OrderManagement.OrderService.Core.Interfaces;
using OrderManagement.Common.Repositories;
using OrderManagement.OrderService.Core.Models;

namespace OrderManagement.OrderService.Data.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {

        public OrderRepository(OrderContext context) : base(context)
        {
        }


    }
}
