using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSiTest.API.Controllers;
using PhotoSiTest.Common.BaseTypes;
using System.ComponentModel.DataAnnotations;

namespace PhotoSiTest.API.Tests.Controller
{
    public class AddressControllerTests
    {
        private readonly Mock<IAddressService> _mockAddressService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly AddressController _controller;

        public AddressControllerTests()
        {
            _mockAddressService = new Mock<IAddressService>();
            _mockOrderService = new Mock<IOrderService>();
            _controller = new AddressController(_mockAddressService.Object, _mockOrderService.Object);
        }

     

        #region GetAddressById Tests

        [Fact]
        public async Task GetAddressById_ReturnsOk_WhenAddressExists()
        {
            // Arrange
            var addressId = 1;
            var expectedAddress = new AddressDto { Id = addressId, Street = "Test Street" };
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync(expectedAddress);

            // Act
            var result = await _controller.GetAddressById(addressId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedAddress);
        }

        [Fact]
        public async Task GetAddressById_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            var addressId = 1;
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync((AddressDto)null);

            // Act
            var result = await _controller.GetAddressById(addressId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetAllAddresses Tests

        [Fact]
        public async Task GetAllAddresses_ReturnsOk_WhenAddressesExist()
        {
            // Arrange
            var addresses = new List<AddressDto>
            {
                new AddressDto { Id = 1, Street = "Test Street" },
                new AddressDto { Id = 2, Street = "Another Street" }
            };
            _mockAddressService.Setup(service => service.GetAllAsync()).ReturnsAsync(addresses);

            // Act
            var result = await _controller.GetAllAddresses();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(addresses);
        }

        [Fact]
        public async Task GetAllAddresses_ReturnsOk_WhenNoAddressesExist()
        {
            // Arrange
            var addresses = new List<AddressDto>();  // Lista vuota
            _mockAddressService.Setup(service => service.GetAllAsync()).ReturnsAsync(addresses);

            // Act
            var result = await _controller.GetAllAddresses();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var value = okResult.Value.Should().BeOfType<List<AddressDto>>().Which;
            value.Should().HaveCount(0);  // Verifica che la lista sia vuota
        }

        #endregion

        #region Paginate Addresses Tests

        [Fact]
        public async Task GetPaginatedAddresses_ShouldReturnOkResult_WhenValidFilterIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var paginatedResult = new PaginatedResult<AddressDto>
            {
                Items = new List<AddressDto>
            {
                new AddressDto { Id = 1, Street = "123 Main St", City = "Sample City" },
                new AddressDto { Id = 2, Street = "456 Elm St", City = "Another City" }
            },
                TotalCount = 50,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            // Mocking the service method
            _mockAddressService.Setup(service => service.PaginateAsync(filter))
                               .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaginatedAddresses(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<AddressDto>>().Which;

            returnValue.Items.Should().HaveCount(2);
            returnValue.TotalCount.Should().Be(50);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetPaginatedAddresses_ShouldReturnBadRequest_WhenInvalidPageNumberIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = -1, // Invalid PageNumber
                PageSize = 10
            };

            // Act
            var result = await _controller.GetPaginatedAddresses(filter);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region CreateAddress Tests

        [Fact]
        public async Task CreateAddress_ReturnsCreatedAtAction_WhenAddressIsCreated()
        {
            // Arrange
            var addressDto = new AddressEditDto { Id = 1, Street = "New Street", City = "New City", Country = "New Country", PostalCode = "New PostalCode" };
            _mockAddressService.Setup(service => service.CreateAsync(addressDto)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateAddress(addressDto);

            // Assert
            result!.Should()!.BeOfType<CreatedAtActionResult>()!
                .Which!.RouteValues["id"]!.Should().Be(addressDto!.Id);
        }

        [Fact]
        public async Task CreateAddress_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var addressDto = new AddressEditDto { Id = 1, Street = "New Street", City = "New City", Country = "New Country", PostalCode = "New PostalCode" };

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.CreateAddress(addressDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task CreateAddress_ShouldReturnBadRequest_WhenDuplicateAddressExists()
        {
            // Arrange
            var addressDto = new AddressEditDto
            {
                Street = "Test Street",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            };

            // Setup mock to simulate existing duplicate address
            _mockAddressService.Setup(x => x.IsDuplicateAddressAsync(addressDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateAddress(addressDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An address with the same values already exists.", badRequestResult.Value);
        }


        #endregion

        #region UpdateAddress Tests

        [Fact]
        public async Task UpdateAddress_ReturnsNoContent_WhenAddressIsUpdated()
        {
            // Arrange
            var addressId = 1;
            var addressDto = new AddressEditDto { Id = addressId, Street = "Updated Street" };
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync(new AddressDto { Id = addressId });
            _mockAddressService.Setup(service => service.UpdateAsync(addressDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAddress(addressId, addressDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateAddress_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var addressDto = new AddressEditDto { Id = 2, Street = "Updated Street" };

            // Act
            var result = await _controller.UpdateAddress(1, addressDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("ID mismatch between route and body.");
        }

        [Fact]
        public async Task UpdateAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            var addressId = 1;
            var addressDto = new AddressEditDto { Id = addressId, Street = "Updated Street" };
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync((AddressDto)null);

            // Act
            var result = await _controller.UpdateAddress(addressId, addressDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Address with ID {addressId} not found.");
        }

        [Fact]
        public async Task UpdateAddress_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var addressDto = new AddressEditDto { Id = 2, Street = "Updated Street" };

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.UpdateAddress(1, addressDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

     
        #endregion

        #region DeleteAddress Tests

        [Fact]
        public async Task DeleteAddress_ReturnsNoContent_WhenAddressIsDeleted()
        {
            // Arrange
            var addressId = 1;
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync(new AddressDto { Id = addressId });
            _mockOrderService.Setup(service => service.HasPendingOrProcessingOrdersWithAddressAsync(addressId)).ReturnsAsync(false);
            _mockAddressService.Setup(service => service.DeleteAsync(addressId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAddress(addressId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            var addressId = 1;
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync((AddressDto)null);

            // Act
            var result = await _controller.DeleteAddress(addressId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Address with ID {addressId} not found.");
        }

        [Fact]
        public async Task DeleteAddress_ReturnsBadRequest_WhenOrdersArePendingOrProcessing()
        {
            // Arrange
            var addressId = 1;
            _mockAddressService.Setup(service => service.GetByIdAsync(addressId)).ReturnsAsync(new AddressDto { Id = addressId });
            _mockOrderService.Setup(service => service.HasPendingOrProcessingOrdersWithAddressAsync(addressId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAddress(addressId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be($"Address with ID {addressId} is associated with orders that are in 'Pending' or 'Processing' state and cannot be deleted.");
        }

        #endregion
    }
}
