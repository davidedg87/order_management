using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.UserService.Core.Dtos;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.API.Controllers;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;
using PhotoSiTest.UserService.Core.Models;

namespace PhotoSiTest.API.Tests.Controller
{
    public class UserControllerTests
    {

        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockOrderService = new Mock<IOrderService>();
            _controller = new UserController(_mockUserService.Object, _mockOrderService.Object);
        }

        #region GetUser

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new UserDto { Id = userId, Name = "Test User", Email = "testuser@example.com" };
            _mockUserService.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeOfType<UserDto>()
                  .Which.Id.Should().Be(userId);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetAllUsers
        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
        {
            // Arrange
            var users = new List<UserDto> { new UserDto { Id = 1, Name = "Test User", Email = "testuser@example.com" } };
            _mockUserService.Setup(service => service.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().BeOfType<List<UserDto>>()
                  .Which.Should().HaveCount(1);
        }


        #endregion

        #region PaginateUsers

        [Fact]
        public async Task GetPaginatedUsers_ShouldReturnOkResult_WhenValidFilterIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var paginatedResult = new PaginatedResult<UserDto>
            {
                Items = new List<UserDto>
            {
                new UserDto { Id = 1, Name = "Test User", Email = "testuser@example.com" },
                new UserDto { Id = 2, Name = "Test User 2", Email = "testuser2@example.com" }
            },
                TotalCount = 50,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            // Mocking the service method
            _mockUserService.Setup(service => service.PaginateAsync(filter))
                               .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaginatedUsers(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<UserDto>>().Which;

            returnValue.Items.Should().HaveCount(2);
            returnValue.TotalCount.Should().Be(50);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetPaginatedUsers_ShouldReturnBadRequest_WhenInvalidPageNumberIsPassed()
        {
            // Arrange
            var filter = new PagedFilter
            {
                PageNumber = -1, // Invalid PageNumber
                PageSize = 10
            };

            // Act
            var result = await _controller.GetPaginatedUsers(filter);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region CreateUser
        [Fact]
        public async Task CreateUser_ReturnsCreatedAtAction_WhenValidData()
        {
            // Arrange
            var userDto = new UserEditDto { Id = 1, Name = "Test User", Email = "testuser@example.com" };
            _mockUserService.Setup(service => service.CreateAsync(It.IsAny<UserEditDto>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateUser(userDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                  .Which.ActionName.Should().Be("GetUserById");
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var userDto = new UserEditDto { Id = 1, Name = "Test User", Email = "testuser@example.com" };
            userDto.Name = string.Empty;

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.CreateUser(userDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenDuplicateUserExists()
        {
            // Arrange
            var userDto = new UserEditDto
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com"
            };

            _mockUserService.Setup(service => service.IsDuplicateUserAsync(userDto))
                .ReturnsAsync(true); // Simula che l'utente sia duplicato

            // Act
            var result = await _controller.CreateUser(userDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("An user with the same values already exists.");
        }

        #endregion

        #region UpdateUser
        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenValidData()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserDto { Id = userId, Name = "Test User", Email = "testuser@example.com" };
            var userEditDto = new UserEditDto { Id = userId, Name = "Test User", Email = "testuser@example.com" };
            _mockUserService.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(userDto);
            _mockUserService.Setup(service => service.UpdateAsync(It.IsAny<UserEditDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(userId, userEditDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var userDto = new UserEditDto { Id = 1, Name = "Test User", Email = "testuser@example.com" };
            userDto.Id = 2;

            // Act
            var result = await _controller.UpdateUser(1, userDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("ID mismatch between route and body.");
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserEditDto { Id = userId, Name = "Test User", Email = "testuser@example.com" };
            _mockUserService.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.UpdateUser(userId, userDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.Value.Should().Be($"User with ID {userId} not found.");
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserEditDto { Id = userId, Name = "Test User", Email = "testuser@example.com" };
            userDto.Name = string.Empty;

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.UpdateUser(userId, userDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserNameIsEmpty()
        {
            // Arrange
            var userDto = new UserEditDto
            {
                Id = 1,
                Name = "",  // Nome utente vuoto
                Email = "valid.email@example.com"
            };

            // Mockiamo il servizio per ottenere un utente esistente
            var existingUser = new UserDto { Id = 1, Name = "Old Name", Email = "old.email@example.com" };
            _mockUserService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.UpdateUser(1, userDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("User name cannot be empty.");  // Verifica che il messaggio sia quello corretto
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            // Arrange
            var userDto = new UserEditDto
            {
                Id = 1,
                Name = "Valid Name",
                Email = "invalid-email"  // Email non valida
            };

            // Mockiamo il servizio per ottenere un utente esistente
            var existingUser = new UserDto { Id = 1, Name = "Old Name", Email = "old.email@example.com" };
            _mockUserService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.UpdateUser(1, userDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("Invalid email address.");  // Verifica che il messaggio sia quello corretto
        }

        #endregion

        #region DeleteUser

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.Value.Should().Be($"User with ID {userId} not found.");
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnBadRequest_WhenUserIsAssociatedWithOrders()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(new UserDto { Id = userId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be($"User with ID {userId} is associated with one or more orders and cannot be deleted.");
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(new UserDto { Id = userId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithUserAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUserService.Verify(s => s.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldNotCallDeleteAsync_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync((UserDto?)null);

            // Act
            await _controller.DeleteUser(userId);

            // Assert
            _mockUserService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_ShouldNotCallDeleteAsync_WhenUserIsAssociatedWithOrders()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(new UserDto { Id = userId });
            _mockOrderService.Setup(s => s.HasPendingOrProcessingOrdersWithUserAsync(userId)).ReturnsAsync(true);

            // Act
            await _controller.DeleteUser(userId);

            // Assert
            _mockUserService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}
