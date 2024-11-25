using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.ProductService.Core.Dtos;


namespace PhotoSi.ProductService.Core.Interfaces
{
    public interface IProductCategoryService : IBaseService<ProductCategory, ProductCategoryDto, ProductCategoryEditDto>
    {
        Task<bool> IsDuplicateProductCategoryAsync(ProductCategoryEditDto productCategoryEditDto);
    }
}