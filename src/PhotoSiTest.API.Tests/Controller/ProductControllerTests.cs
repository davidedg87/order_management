using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSiTest.API.Controllers;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.API.Tests.Controller
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IProductCategoryService> _mockProductCategoryService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockProductCategoryService = new Mock<IProductCategoryService>();
            _mockOrderService = new Mock<IOrderService>();
            _controller = new ProductController(_mockProductService.Object, _mockProductCategoryService.Object, _mockOrderService.Object);
        }

        #region GetProduct

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var product = new ProductDto { Id = productId, Name = "TestProduct", Price = 10.0m};
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(product);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync((ProductDto)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetAllProducts

        [Fact]
        public async Task GetAllProducts_ShouldReturnOkWithProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Product1", Price = 10.0m},
                new ProductDto { Id = 2, Name = "Product2", Price = 20.0m}
            };
            _mockProductService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(products);
        }

        #endregion

        #region PaginateProducts

        [Fact]
        public async Task GetPaginatedProducts_ShouldReturnOkResult_WhenValidFilterIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var paginatedResult = new PaginatedResult<ProductDto>
            {
                Items = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Product1", Price = 10.0m},
                new ProductDto { Id = 2, Name = "Product2", Price = 20.0m}
            },
                TotalCount = 50,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            // Mocking the service method
            _mockProductService.Setup(service => service.PaginateAsync(filter))
                               .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaginatedProducts(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<ProductDto>>().Which;

            returnValue.Items.Should().HaveCount(2);
            returnValue.TotalCount.Should().Be(50);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetPaginatedProducts_ShouldReturnBadRequest_WhenInvalidPageNumberIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = -1, // Invalid PageNumber
                PageSize = 10
            };

            // Act
            var result = await _controller.GetPaginatedProducts(filter);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion


        #region CreateProduct

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedAtAction_WhenProductIsCreated()
        {
            // Arrange
            var product = new ProductEditDto { Id = 1, Name = "Product1", Price = 10.0m, ProductCategoryId = 1, Description = "Product1 Description" };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(product.ProductCategoryId)).ReturnsAsync(new ProductCategoryDto());
            _mockProductService.Setup(s => s.CreateAsync(product)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.RouteValues["id"].Should().Be(product.Id);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
        {
            // Arrange
            var product = new ProductEditDto { Id = 1, Name = "Product1", Price = 10.0m, ProductCategoryId = 99 };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(product.ProductCategoryId)).ReturnsAsync((ProductCategoryDto)null);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be($"Category with ID {product.ProductCategoryId} does not exist.");
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenPriceIsNegative()
        {
            // Arrange
            var product = new ProductEditDto { Id = 1, Name = "Product1", Price = -5.0m, ProductCategoryId = 1 };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(product.ProductCategoryId)).ReturnsAsync(new ProductCategoryDto());

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("The product price cannot be less than 0.");
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidModel()
        {
            // Arrange
            var product = new ProductEditDto { Id = 1, Name = "Product1", Price = 10.0m, ProductCategoryId = 99 };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(product.ProductCategoryId)).ReturnsAsync((ProductCategoryDto)null);

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");


            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenDuplicateProductExists()
        {
            // Arrange
            var productDto = new ProductEditDto
            {
                Id = 1,
                Name = "Product 1",
                ProductCategoryId = 1,
                Price = 10
            };

            _mockProductService.Setup(service => service.IsDuplicateProductAsync(productDto))
                .ReturnsAsync(true); // Simula che il prodotto sia duplicato

            // Act
            var result = await _controller.CreateProduct(productDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("A product with the same values already exists.");
        }

        #endregion

        #region UpdateProduct

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenProductIdDoesNotMatchDtoId()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = 2 }; // ID nel DTO diverso dall'ID nell'URL

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("ID mismatch between route and body.");
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = id, ProductCategoryId = 1 };
            _mockProductService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((ProductDto)null); // Il prodotto non esiste

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Product with ID {id} not found.");
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = id, ProductCategoryId = 999 }; // Categoria inesistente
            _mockProductService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new ProductDto()); // Prodotto esistente
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ProductCategoryDto)null); // Categoria non trovata

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be($"Category with ID {productDto.ProductCategoryId} does not exist.");
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenPriceIsNegative()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = id, ProductCategoryId = 1, Price = -5 }; // Prezzo negativo
            _mockProductService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new ProductDto()); // Prodotto esistente
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ProductCategoryDto()); // Categoria esistente

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("The product price cannot be less than 0.");
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent_WhenProductIsUpdatedSuccessfully()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = id, ProductCategoryId = 1, Price = 10 };
            _mockProductService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new ProductDto()); // Prodotto esistente
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ProductCategoryDto()); // Categoria esistente

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenInvalidModel()
        {
            // Arrange
            var id = 1;
            var productDto = new ProductEditDto { Id = 2 }; // ID nel DTO diverso dall'ID nell'URL

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.UpdateProduct(id, productDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        #endregion

        #region DeleteProduct
        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.Value.Should().Be($"Product with ID {productId} not found.");
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnBadRequest_WhenProductIsAssociatedWithOrders()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync(new ProductDto { Id = productId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithProductAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be($"Product with ID {productId} is associated with one or more orders and cannot be deleted.");
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenProductIsDeletedSuccessfully()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync(new ProductDto { Id = productId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithProductAsync(productId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockProductService.Verify(s => s.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldNotCallDeleteAsync_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync((ProductDto?)null);

            // Act
            await _controller.DeleteProduct(productId);

            // Assert
            _mockProductService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProduct_ShouldNotCallDeleteAsync_WhenProductIsAssociatedWithOrders()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync(new ProductDto { Id = productId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithProductAsync(productId)).ReturnsAsync(true);

            // Act
            await _controller.DeleteProduct(productId);

            // Assert
            _mockProductService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

    }
}
