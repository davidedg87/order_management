using OrderManagement.ProductService.Core.Interfaces;
using OrderManagement.ProductService.Core.Models;
using OrderManagement.ProductService.Data;
using OrderManagement.Common.Repositories;

namespace OrderManagement.ProductService.Data.Repositories
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
    {

        public ProductCategoryRepository(ProductContext context) : base(context)
        {
        }


    }
}
