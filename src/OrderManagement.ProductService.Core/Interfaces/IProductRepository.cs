using OrderManagement.ProductService.Core.Models;
using OrderManagement.Common.Interfaces;

namespace OrderManagement.ProductService.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
    }
}