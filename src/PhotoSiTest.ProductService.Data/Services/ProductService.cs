using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.Common.Services;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSi.ProductService.Services
{
    public class ProductService : BaseService<Product, ProductDto, ProductEditDto>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger) : base(productRepository, logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<bool> IsDuplicateProductAsync(ProductEditDto productDto)
        {
            _logger.LogTrace("Checking if product exists for Name: {Name}, ProductCategoryId: {ProductCategoryId}",
                productDto.Name, productDto.ProductCategoryId);

            // Verifica se esiste già un prodotto con gli stessi valori
            var existingProduct = await _productRepository.Query()
                .FirstOrDefaultAsync(p => p.Name == productDto.Name &&
                                          p.ProductCategoryId == productDto.ProductCategoryId);

            return existingProduct != null; // Restituisce true se esiste un duplicato
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId)
        {
            _logger.LogTrace("Fetching products for Category ID: {CategoryId}", categoryId);

            var products = await _productRepository.Query()
                                                    .Where(p => p.ProductCategoryId == categoryId)
                                                    .Select(p => p.Adapt<ProductDto>())
                                                    .ToListAsync();

            _logger.LogTrace("Found {ProductCount} products for Category ID: {CategoryId}", products.Count(), categoryId);
            return products;
        }

        public override async Task<ProductDto?> GetByIdAsync(int id)
        {
            _logger.LogTrace("Fetching product with ID: {ProductId}", id);

            var product = await _productRepository.Query()
                                                   .Include(p => p.ProductCategory)
                                                   .Where(entity => entity.Id == id)
                                                   .Select(entity => entity.Adapt<ProductDto>())
                                                   .FirstOrDefaultAsync();

            if (product != null)
            {
                _logger.LogTrace("Found product with ID: {ProductId}", id);
            }
            else
            {
                _logger.LogTrace("Product with ID: {ProductId} not found", id);
            }

            return product;
        }

        public override async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            _logger.LogTrace("Fetching all products.");

            var products = await _productRepository.Query()
                                                    .Include(p => p.ProductCategory)
                                                    .Select(entity => entity.Adapt<ProductDto>())
                                                    .ToListAsync();

            _logger.LogTrace("Found {ProductCount} products.", products.Count);
            return products;
        }


        public override async Task<PaginatedResult<ProductDto>> PaginateAsync(PagedFilter filter)
        {
            _logger.LogTrace("Fetching all products.");

            if (filter.PageNumber <= 0)
                throw new ArgumentException("PageNumber must be greater than zero.", nameof(filter.PageNumber));

            if (filter.PageSize <= 0)
                throw new ArgumentException("PageSize must be greater than zero.", nameof(filter.PageSize));

            // Esegui la query sul repository
            var query = _productRepository.Query();

            // Conta il totale degli elementi
            var totalCount = await query.CountAsync();

            // Recupera gli elementi paginati
            var items = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .Select(entity => entity.Adapt<ProductDto>())
                                   .ToListAsync();

            // Costruisci il risultato
            return new PaginatedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

        }

        public async Task<IEnumerable<ProductDto>> GetByIdsAsync(List<int> productIds)
        {
            _logger.LogTrace("Fetching products for IDs: {ProductIds}", string.Join(", ", productIds));

            var products = await _productRepository.Query()
                                                    .Where(a => productIds.Contains(a.Id))
                                                    .Select(p => p.Adapt<ProductDto>())
                                                    .ToListAsync();

            _logger.LogTrace("Found {ProductCount} products for the provided IDs.", products.Count());
            return products;
        }

        public async Task<decimal> GetSumAmount(List<int> productIds)
        {
            _logger.LogTrace("Calculating sum of prices for products with IDs: {ProductIds}", string.Join(", ", productIds));

            var sum = await _productRepository.Query()
                                               .Where(a => productIds.Contains(a.Id))
                                               .SumAsync(x => x.Price);

            _logger.LogTrace("Total sum of prices for products with IDs {ProductIds}: {SumAmount}", string.Join(", ", productIds), sum);
            return sum;
        }

        public async Task<List<ProductCodeDto>> GetProductCodesByIdsAsync(List<int> productIds)
        {
            _logger.LogTrace("Fetching product codes for product IDs: {ProductIds}", string.Join(", ", productIds));

            var productCodes = await _productRepository.Query()
                                                       .Where(p => productIds.Contains(p.Id))
                                                       .Select(p => new ProductCodeDto
                                                       {
                                                           ProductId = p.Id,
                                                           Code = p.Name
                                                       })
                                                       .ToListAsync();

            _logger.LogTrace("Found {ProductCodeCount} product codes for provided product IDs.", productCodes.Count);
            return productCodes;
        }
    }
}
