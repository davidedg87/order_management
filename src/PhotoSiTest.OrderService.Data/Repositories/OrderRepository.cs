using PhotoSi.OrderService.Core.Interfaces;
using PhotoSiTest.Common.Repositories;
using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSiTest.OrderService.Data.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {

        public OrderRepository(OrderContext context) : base(context)
        {
        }


    }
}
