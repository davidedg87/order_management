using OrderManagement.ProductService.Core.Models;
using OrderManagement.Common.Interfaces;
using OrderManagement.ProductService.Core.Dtos;


namespace OrderManagement.ProductService.Core.Interfaces
{
    public interface IProductCategoryService : IBaseService<ProductCategory, ProductCategoryDto, ProductCategoryEditDto>
    {
        Task<bool> IsDuplicateProductCategoryAsync(ProductCategoryEditDto productCategoryEditDto);
    }
}