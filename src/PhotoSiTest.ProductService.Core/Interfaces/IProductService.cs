using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.ProductService.Core.Dtos;


namespace PhotoSi.ProductService.Core.Interfaces
{
    public interface IProductService : IBaseService<Product, ProductDto, ProductEditDto>
    {
        Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId);

        Task<IEnumerable<ProductDto>> GetByIdsAsync(List<int> productIds);

        Task<List<ProductCodeDto>> GetProductCodesByIdsAsync(List<int> productIds);

        Task<decimal> GetSumAmount(List<int> productIds);

        Task<bool> IsDuplicateProductAsync(ProductEditDto productDto);
    }
}