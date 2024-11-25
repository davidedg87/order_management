using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using PhotoSi.UserService.Core.Dtos;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.UserService.Core.Models;

namespace PhotoSiTest.UserService.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<PhotoSi.UserService.Services.UserService>> _loggerMock;
        private readonly PhotoSi.UserService.Services.UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<PhotoSi.UserService.Services.UserService>>();
            _userService = new PhotoSi.UserService.Services.UserService(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnUserDtos_WhenUsersExist()
        {
            // Arrange
            var userIds = new List<int> { 1, 2, 3 };
            var users = new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new User { Id = 2, Name = "Jane Doe", Email = "jane@example.com" },
                new User { Id = 3, Name = "Jim Doe", Email = "jim@example.com" }
            };

            _userRepositoryMock.Setup(repo => repo.Query(false))
                               .Returns(users.AsQueryable().BuildMock());

            // Act
            var result = await _userService.GetByIdsAsync(userIds);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.First().Should().BeOfType<UserDto>();
            result.First().Name.Should().Be("John Doe");
            result.First().Email.Should().Be("john@example.com");

        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            // Arrange
            var userIds = new List<int> { 99, 100 }; // Assume these users do not exist
            var users = new List<User>();

            _userRepositoryMock.Setup(repo => repo.Query(false))
                               .Returns(users.AsQueryable().BuildMock());

            // Act
            var result = await _userService.GetByIdsAsync(userIds);

            // Assert
            result.Should().BeEmpty();

        }

        [Fact]
        public async Task IsDuplicateUserAsync_ShouldReturnTrue_WhenUserWithSameEmailExists()
        {
            var userEditDto = new UserEditDto
            {
                Email = "test@example.com"
            };

            var existingUser = new User
            {
                Email = "test@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.Query(false))
                .Returns(new List<User> { existingUser }.AsQueryable().BuildMock());

            var result = await _userService.IsDuplicateUserAsync(userEditDto);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsDuplicateUserAsync_ShouldReturnFalse_WhenUserWithSameEmailDoesNotExist()
        {
            var userEditDto = new UserEditDto
            {
                Email = "test@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.Query(false))
                .Returns(new List<User>().AsQueryable().BuildMock());

            var result = await _userService.IsDuplicateUserAsync(userEditDto);

            result.Should().BeFalse();
        }

    }
}
