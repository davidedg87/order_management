using OrderManagement.ProductService.Core.Interfaces;
using OrderManagement.ProductService.Core.Models;
using OrderManagement.ProductService.Data;
using OrderManagement.Common.Repositories;

namespace OrderManagement.ProductService.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {

        public ProductRepository(ProductContext context) : base(context)
        {
        }


    }
}
