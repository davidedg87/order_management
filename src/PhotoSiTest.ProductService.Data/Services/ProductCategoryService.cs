using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.Services;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSi.ProductCategoryService.Services
{
    public class ProductCategoryService : BaseService<ProductCategory, ProductCategoryDto, ProductCategoryEditDto>, IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly ILogger<ProductCategoryService> _logger;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository, ILogger<ProductCategoryService> logger) : base(productCategoryRepository, logger)
        {
            _productCategoryRepository = productCategoryRepository;
            _logger = logger;
        }

        public async Task<bool> IsDuplicateProductCategoryAsync(ProductCategoryEditDto productCategoryEditDto)
        {
            _logger.LogTrace("Checking if product category exists for Name: {Name}, Description: {Description}", productCategoryEditDto.Name, productCategoryEditDto.Description);

            // Verifica se esiste già una categoria con gli stessi valori di Name e Description
            var existingCategory = await _productCategoryRepository.Query()
                .FirstOrDefaultAsync(c => c.Name == productCategoryEditDto.Name && c.Description == productCategoryEditDto.Description);

            return existingCategory != null; // Restituisce true se esiste un duplicato
        }

    }
}
