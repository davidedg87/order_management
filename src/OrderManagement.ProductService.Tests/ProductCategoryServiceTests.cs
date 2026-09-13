using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OrderManagement.ProductService.Core.Interfaces;
using OrderManagement.ProductService.Core.Models;
using OrderManagement.ProductService.Core.Dtos;

namespace OrderManagement.ProductCategoryService.Tests
{
    public class ProductCategoryServiceTests
    {
        private readonly Mock<IProductCategoryRepository> _mockRepository;
        private readonly Mock<ILogger<OrderManagement.ProductCategoryService.Services.ProductCategoryService>> _mockLogger;
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryServiceTests()
        {
            _mockRepository = new Mock<IProductCategoryRepository>();
            _mockLogger = new Mock<ILogger<OrderManagement.ProductCategoryService.Services.ProductCategoryService>>();
            _productCategoryService = new OrderManagement.ProductCategoryService.Services.ProductCategoryService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task IsDuplicateProductCategoryAsync_ShouldReturnTrue_WhenProductCategoryExists()
        {
            var productCategoryEditDto = new ProductCategoryEditDto
            {
                Name = "Electronics",
                Description = "All electronic devices"
            };

            var existingCategory = new ProductCategory
            {
                Name = "Electronics",
                Description = "All electronic devices"
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<ProductCategory> { existingCategory }.AsQueryable().BuildMock());

            var result = await _productCategoryService.IsDuplicateProductCategoryAsync(productCategoryEditDto);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsDuplicateProductCategoryAsync_ShouldReturnFalse_WhenProductCategoryDoesNotExist()
        {
            var productCategoryEditDto = new ProductCategoryEditDto
            {
                Name = "Electronics",
                Description = "All electronic devices"
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<ProductCategory>().AsQueryable().BuildMock());

            var result = await _productCategoryService.IsDuplicateProductCategoryAsync(productCategoryEditDto);

            result.Should().BeFalse();
        }


    }
}
