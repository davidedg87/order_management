using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.ProductService.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<PhotoSi.ProductService.Services.ProductService>> _mockLogger;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<PhotoSi.ProductService.Services.ProductService>>();
            _productService = new PhotoSi.ProductService.Services.ProductService(_mockRepository.Object, _mockLogger.Object);
        }

        //INSERISCI TEST SPECIFICI PER PRODUCT SERVICE NON COMPRESI NEI TEST DI BASE SERVICE

        [Fact]
        public async Task GetByCategoryIdAsync_ShouldReturnProductDtos_WhenProductsExistInCategory()
        {
            // Arrange
            var categoryId = 1;
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", ProductCategoryId = categoryId },
                new Product { Id = 2, Name = "Product 2", ProductCategoryId = categoryId }
            };

            var productDtos = products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, ProductCategoryId = categoryId }).ToList();

            // Mock repository query to return products based on category
            _mockRepository.Setup(r => r.Query(false)).Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _productService.GetByCategoryIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(productDtos);
        }

        [Fact]
        public async Task GetByCategoryIdAsync_ShouldReturnEmptyList_WhenNoProductsExistInCategory()
        {
            // Arrange
            var categoryId = 1;
            _mockRepository.Setup(r => r.Query(false)).Returns(Enumerable.Empty<Product>().AsQueryable().BuildMock());

            // Act
            var result = await _productService.GetByCategoryIdAsync(categoryId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsProductDto()
        {
            // Arrange
            var productId = 1;
            var product = new Product
            {
                Id = productId,
                ProductCategory = new ProductCategory { Id = 1, Name = "Category 1" }
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new[] { product }.AsQueryable().BuildMock());

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.ProductCategoryName.Should().Be("Category 1");
        }

        [Fact]
        public async Task GetByIdAsync_ProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            var productId = 1;

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(Enumerable.Empty<Product>().AsQueryable().BuildMock());

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 100, ProductCategoryId = 1, Description = "Description1" },
            new Product { Id = 2, Name = "Product2", Price = 200, ProductCategoryId = 2, Description = "Description2" }
        }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(products.BuildMock());

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.HaveCount(2)
                .And.ContainSingle(p => p.Name == "Product1")
                .And.ContainSingle(p => p.Name == "Product2");

        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnProducts_WhenValidIdsProvided()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 100 },
            new Product { Id = 2, Name = "Product2", Price = 200 }
        }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(products.BuildMock());

            // Act
            var result = await _productService.GetByIdsAsync(new List<int> { 1, 2 });

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.HaveCount(2)
                .And.ContainSingle(p => p.Name == "Product1")
                .And.ContainSingle(p => p.Name == "Product2");

        }

        [Fact]
        public async Task GetSumAmount_ShouldReturnSumOfPrices_WhenValidIdsProvided()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Price = 100 },
            new Product { Id = 2, Price = 200 }
        }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(products.BuildMock());

            // Act
            var result = await _productService.GetSumAmount(new List<int> { 1, 2 });

            // Assert
            result.Should().Be(300);

        }

        [Fact]
        public async Task GetProductCodesByIdsAsync_ShouldReturnProductCodes_WhenValidIdsProvided()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Code1" },
                new Product { Id = 2, Name = "Code2" }
            }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(products.BuildMock());

            // Act
            var result = await _productService.GetProductCodesByIdsAsync(new List<int> { 1, 2 });

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.HaveCount(2)
                .And.ContainSingle(p => p.ProductId == 1 && p.Code == "Code1")
                .And.ContainSingle(p => p.ProductId == 2 && p.Code == "Code2");

        }


        [Fact]
        public async Task PaginateAsync_ShouldReturnPaginatedResult_WhenFilterIsValid()
        {
            var testData = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10 },
                new Product { Id = 2, Name = "Product 2", Price = 20 },
                new Product { Id = 3, Name = "Product 3", Price = 30 },
                new Product { Id = 4, Name = "Product 4", Price = 40 },
                new Product { Id = 5, Name = "Product 5", Price = 50 }
            }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(testData.BuildMock());


            var filter = new PagedFilter { PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _productService.PaginateAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(5); // Total items in the data
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items.First().Name.Should().Be("Product 1");
            result.Items.Last().Name.Should().Be("Product 2");
        }


        [Fact]
        public async Task PaginateAsync_ShouldThrowArgumentException_WhenPageNumberIsInvalid()
        {
            // Arrange
            var filter = new PagedFilter { PageNumber = 0, PageSize = 2 };

            // Act
            Func<Task> act = async () => await _productService.PaginateAsync(filter);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*PageNumber must be greater than zero*");
        }

        [Fact]
        public async Task PaginateAsync_ShouldThrowArgumentException_WhenPageSizeIsInvalid()
        {
            // Arrange
            var filter = new PagedFilter { PageNumber = 1, PageSize = 0 };

            // Act
            Func<Task> act = async () => await _productService.PaginateAsync(filter);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*PageSize must be greater than zero*");
        }

        [Fact]
        public async Task IsDuplicateProductAsync_ShouldReturnTrue_WhenProductExists()
        {
            var productDto = new ProductEditDto
            {
                Name = "Test Product",
                ProductCategoryId = 1
            };

            var existingProduct = new Product
            {
                Name = "Test Product",
                ProductCategoryId = 1
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Product> { existingProduct }.AsQueryable().BuildMock());

            var result = await _productService.IsDuplicateProductAsync(productDto);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsDuplicateProductAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var productDto = new ProductEditDto
            {
                Name = "Test Product",
                ProductCategoryId = 1
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Product>().AsQueryable().BuildMock());

            var result = await _productService.IsDuplicateProductAsync(productDto);

            result.Should().BeFalse();
        }
    }

}

