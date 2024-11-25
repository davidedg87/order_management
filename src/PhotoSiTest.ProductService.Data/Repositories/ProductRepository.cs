using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.ProductService.Core.Models;
using PhotoSi.ProductService.Data;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.ProductService.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {

        public ProductRepository(ProductContext context) : base(context)
        {
        }


    }
}
