using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSiTest.API.Controllers;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.API.Tests.Controller
{
    public class ProductCategoryControllerTests
    {
        private readonly Mock<IProductCategoryService> _mockProductCategoryService;
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductCategoryController _controller;

        public ProductCategoryControllerTests()
        {
            _mockProductCategoryService = new Mock<IProductCategoryService>();
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductCategoryController(_mockProductCategoryService.Object, _mockProductService.Object);
        }

        #region GetProductCategory
        [Fact]
        public async Task GetProductCategoryById_ShouldReturnOk_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var category = new ProductCategoryDto { Id = categoryId, Name = "TestCategory", Description = "TestDescription" };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetProductCategoryById(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(category);
        }

        [Fact]
        public async Task GetProductCategoryById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 1;
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(categoryId)).ReturnsAsync((ProductCategoryDto)null);

            // Act
            var result = await _controller.GetProductCategoryById(categoryId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetAllProductCategories

        [Fact]
        public async Task GetAllProductCategories_ShouldReturnOkWithCategories_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<ProductCategoryDto>
            {
                new ProductCategoryDto { Id = 1, Name = "Category1", Description = "Description1" },
                new ProductCategoryDto { Id = 2, Name = "Category2", Description = "Description2" }
            };
            _mockProductCategoryService.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllProductCategories();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(categories);
        }

        #endregion

        #region PaginateProductCategories

        [Fact]
        public async Task GetPaginatedProductCategories_ShouldReturnOkResult_WhenValidFilterIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var paginatedResult = new PaginatedResult<ProductCategoryDto>
            {
                Items = new List<ProductCategoryDto>
            {
                new ProductCategoryDto { Id = 1, Name = "Category1", Description = "Description1" },
                new ProductCategoryDto { Id = 2, Name = "Category2", Description = "Description2" }
            },
                TotalCount = 50,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            // Mocking the service method
            _mockProductCategoryService.Setup(service => service.PaginateAsync(filter))
                               .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaginatedProductCategories(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<ProductCategoryDto>>().Which;

            returnValue.Items.Should().HaveCount(2);
            returnValue.TotalCount.Should().Be(50);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetPaginatedProductCategories_ShouldReturnBadRequest_WhenInvalidPageNumberIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = -1, // Invalid PageNumber
                PageSize = 10
            };

            // Act
            var result = await _controller.GetPaginatedProductCategories(filter);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region CreateProductCategory

        [Fact]
        public async Task CreateProductCategory_ShouldReturnCreatedAtAction_WhenCategoryIsCreated()
        {
            // Arrange
            var category = new ProductCategoryEditDto { Id = 1, Name = "Category1", Description = "Description1" };
            _mockProductCategoryService.Setup(s => s.CreateAsync(category)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateProductCategory(category);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.RouteValues["id"].Should().Be(category.Id);
        }

        [Fact]
        public async Task CreateProductCategory_ShouldReturnBadRequest_WhenInvalidModel()
        {
            // Arrange
            var category = new ProductCategoryEditDto { Id = 1, Name = "Category1", Description = "Description1" };
            _mockProductCategoryService.Setup(s => s.CreateAsync(category)).ReturnsAsync(1);

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.CreateProductCategory(category);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task CreateProductCategory_ShouldReturnBadRequest_WhenDuplicateCategoryExists()
        {
            // Arrange
            var categoryDto = new ProductCategoryEditDto
            {
                Id = 1,
                Name = "Category 1"
            };

            _mockProductCategoryService.Setup(service => service.IsDuplicateProductCategoryAsync(categoryDto))
                .ReturnsAsync(true); // Simula che la categoria sia duplicata

            // Act
            var result = await _controller.CreateProductCategory(categoryDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("A product category with the same values already exists.");
        }

        #endregion

        #region UpdateProductCategory

        [Fact]
        public async Task UpdateProductCategory_ShouldReturnBadRequest_WhenIdsDontMatch()
        {
            // Arrange
            var categoryDto = new ProductCategoryEditDto { Id = 2 }; // ID mismatch
            var idInRoute = 1;

            // Act
            var result = await _controller.UpdateProductCategory(idInRoute, categoryDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("ID mismatch between route and body.");
        }

        [Fact]
        public async Task UpdateProductCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryDto = new ProductCategoryEditDto { Id = 1 };
            var idInRoute = 1;

            _mockProductCategoryService.Setup(s => s.GetByIdAsync(idInRoute))
                                       .ReturnsAsync((ProductCategoryDto)null); // Simuliamo che la categoria non esista

            // Act
            var result = await _controller.UpdateProductCategory(idInRoute, categoryDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Category with ID 1 not found.");
        }

        [Fact]
        public async Task UpdateProductCategory_ShouldReturnNoContent_WhenCategoryExistsAndUpdated()
        {
            // Arrange
            var categoryDto = new ProductCategoryDto { Id = 1 };
            var categoryEditDto = new ProductCategoryEditDto { Id = 1 };
            var idInRoute = 1;

            _mockProductCategoryService.Setup(s => s.GetByIdAsync(idInRoute))
                                       .ReturnsAsync(categoryDto); // Simuliamo che la categoria esista

            // Act
            var result = await _controller.UpdateProductCategory(idInRoute, categoryEditDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateProductCategory_ShouldReturnBadRequest_WhenInvalidModel()
        {
            // Arrange
            var categoryDto = new ProductCategoryEditDto { Id = 2 }; // ID mismatch
            var idInRoute = 1;

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.UpdateProductCategory(idInRoute, categoryDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }


        #endregion

        #region Delete ProductCategory
        [Fact]
        public async Task DeleteProductCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 1;
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(id))
                                       .ReturnsAsync((ProductCategoryDto)null); // Simuliamo che la categoria non esista

            // Act
            var result = await _controller.DeleteProductCategory(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Category with ID {id} not found.");
        }

        [Fact]
        public async Task DeleteProductCategory_ShouldReturnBadRequest_WhenCategoryHasProducts()
        {
            // Arrange
            var id = 1;
            var existingCategory = new ProductCategoryDto { Id = id };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(id))
                                       .ReturnsAsync(existingCategory); // Simuliamo che la categoria esista

            var productsInCategory = new List<ProductDto> { new ProductDto() }; // Simuliamo che ci siano prodotti associati
            _mockProductService.Setup(s => s.GetByCategoryIdAsync(id))
                               .ReturnsAsync(productsInCategory); // Simuliamo che ci siano prodotti

            // Act
            var result = await _controller.DeleteProductCategory(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Cannot delete category because there are products associated with it.");
        }

        [Fact]
        public async Task DeleteProductCategory_ShouldReturnNoContent_WhenCategoryIsDeleted()
        {
            // Arrange
            var id = 1;
            var existingCategory = new ProductCategoryDto { Id = id };
            _mockProductCategoryService.Setup(s => s.GetByIdAsync(id))
                                       .ReturnsAsync(existingCategory); // Simuliamo che la categoria esista

            var productsInCategory = new List<ProductDto>(); // Simuliamo che non ci siano prodotti associati
            _mockProductService.Setup(s => s.GetByCategoryIdAsync(id))
                               .ReturnsAsync(productsInCategory); // Simuliamo che non ci siano prodotti

            // Act
            var result = await _controller.DeleteProductCategory(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        #endregion
    }
}
