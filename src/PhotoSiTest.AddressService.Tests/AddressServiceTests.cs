using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.AddressService.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PhotoSi.AddressService.Tests
{
    public class AddressServiceTests
    {
        private readonly Mock<IAddressRepository> _mockRepository;
        private readonly Mock<ILogger<AddressService.Services.AddressService>> _mockLogger;
        private readonly IAddressService _addressService;

        public AddressServiceTests()
        {
            _mockRepository = new Mock<IAddressRepository>();
            _mockLogger = new Mock<ILogger<AddressService.Services.AddressService>>();
            _addressService = new AddressService.Services.AddressService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnAddresses_WhenValidIdsAreProvided()
        {
            // Arrange
            var addressIds = new List<int> { 1, 2 };
            var addresses = new List<Address>
        {
            new Address { Id = 1, Street ="street", City = "city", Country = "country"},
            new Address { Id = 2, Street ="street2", City = "city2", Country = "country2"},
        };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(addresses.AsQueryable().BuildMock());

            // Act
            var result = await _addressService.GetByIdsAsync(addressIds);

            // Assert
            result.Should().HaveCount(2);
            result.First().Street.Should().Be($"{addresses[0].Street}");
            result.First().City.Should().Be($"{addresses[0].City}");
            result.First().Country.Should().Be($"{addresses[0].Country}");
            result.Last().Street.Should().Be($"{addresses[1].Street}");
            result.Last().City.Should().Be($"{addresses[1].City}");
            result.Last().Country.Should().Be($"{addresses[1].Country}");
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenNoIdsExist()
        {
            // Arrange
            var addressIds = new List<int> { 3, 4 };
            var addresses = new List<Address>(); // No addresses in the repository

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(addresses.AsQueryable().BuildMock());

            // Act
            var result = await _addressService.GetByIdsAsync(addressIds);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenIdsListIsEmpty()
        {
            // Arrange
            var addressIds = new List<int>(); // Empty list
            var addresses = new List<Address>();

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(addresses.AsQueryable().BuildMock());

            // Act
            var result = await _addressService.GetByIdsAsync(addressIds);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task IsDuplicateAddressAsync_ShouldReturnTrue_WhenAddressExists()
        {
            // Arrange
            var addressDto = new AddressEditDto
            {
                Street = "123 Main St",
                City = "Sample City",
                PostalCode = "12345",
                Country = "Sample Country"
            };

            var existingAddress = new Address
            {
                Street = "123 Main St",
                City = "Sample City",
                PostalCode = "12345",
                Country = "Sample Country"
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Address> { existingAddress }.AsQueryable().BuildMock()); // Simula un indirizzo esistente

            // Act
            var result = await _addressService.IsDuplicateAddressAsync(addressDto);

            // Assert
            result.Should().BeTrue(); // Dovrebbe restituire true, poiché l'indirizzo esiste già
        }

        [Fact]
        public async Task IsDuplicateAddressAsync_ShouldReturnFalse_WhenAddressDoesNotExist()
        {
            // Arrange
            var addressDto = new AddressEditDto
            {
                Street = "123 Main St",
                City = "Sample City",
                PostalCode = "12345",
                Country = "Sample Country"
            };

            _mockRepository.Setup(repo => repo.Query(false))
                .Returns(new List<Address>().AsQueryable().BuildMock()); // Simula nessun indirizzo esistente

            // Act
            var result = await _addressService.IsDuplicateAddressAsync(addressDto);

            // Assert
            result.Should().BeFalse(); // Dovrebbe restituire false, poiché l'indirizzo non esiste
        }



    }
}
