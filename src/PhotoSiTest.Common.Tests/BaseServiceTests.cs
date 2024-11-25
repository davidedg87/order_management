using FluentAssertions;
using Mapster;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.Common.Services;

namespace PhotoSiTest.Common.Tests
{
    public class BaseServiceTests
    {
        private readonly Mock<IBaseRepository<TestEntity>> _mockRepository;
        private readonly Mock<ILogger<BaseService<TestEntity, TestEntityDto, TestEntityEditDto>>> _mockLogger;
        private readonly BaseService<TestEntity, TestEntityDto, TestEntityEditDto> _service;

        public BaseServiceTests()
        {
            _mockRepository = new Mock<IBaseRepository<TestEntity>>();
            _mockLogger = new Mock<ILogger<BaseService<TestEntity, TestEntityDto, TestEntityEditDto>>>();
            _service = new BaseService<TestEntity, TestEntityDto, TestEntityEditDto>(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenEntityExists()
        {
            // Arrange
            var entityId = 1;
            var entity = new TestEntity { Id = entityId, Name = "Test Entity" };
            var dto = new TestEntityDto { Id = entityId, Name = "Test Entity" };

            _mockRepository.Setup(r => r.Query(false)).Returns(new List<TestEntity> { entity }.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetByIdAsync(entityId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var entityId = 1;
            _mockRepository.Setup(r => r.Query(false)).Returns(Enumerable.Empty<TestEntity>().AsQueryable().BuildMock());

            // Act
            var result = await _service.GetByIdAsync(entityId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDtos_WhenEntitiesExist()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Entity 1" },
                new TestEntity { Id = 2, Name = "Entity 2" }
            };

            var dtos = entities.Select(e => new TestEntityDto { Id = e.Id, Name = e.Name }).ToList();

            _mockRepository.Setup(r => r.Query(false)).Returns(entities.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEntitiesExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.Query(false)).Returns(Enumerable.Empty<TestEntity>().AsQueryable().BuildMock());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }



        [Fact]
        public async Task PaginateAsync_ShouldReturnPaginatedResult_WhenFilterIsValid()
        {
            var testData = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Entity 1" },
                new TestEntity { Id = 2, Name = "Entity 2" },
                new TestEntity { Id = 3, Name = "Entity 3" },
                new TestEntity { Id = 4, Name = "Entity 4" },
                new TestEntity { Id = 5, Name = "Entity 5" }
            }.AsQueryable();

            _mockRepository.Setup(r => r.Query(false)).Returns(testData.BuildMock());


            var filter = new PagedFilter { PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _service.PaginateAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(5); // Total items in the data
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items.First().Name.Should().Be("Entity 1");
            result.Items.Last().Name.Should().Be("Entity 2");
        }


        [Fact]
        public async Task PaginateAsync_ShouldThrowArgumentException_WhenPageNumberIsInvalid()
        {
            // Arrange
            var filter = new PagedFilter { PageNumber = 0, PageSize = 2 };

            // Act
            Func<Task> act = async () => await _service.PaginateAsync(filter);

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
            Func<Task> act = async () => await _service.PaginateAsync(filter);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*PageSize must be greater than zero*");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEntity_WhenDtoIsValid()
        {
            // Arrange
            var dto = new TestEntityEditDto { Id = 1, Name = "New Entity" };
            var entity = dto.Adapt<TestEntity>();

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TestEntity>())).Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<TestEntity>(e => e.Name == "New Entity")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity_WhenDtoIsValid()
        {
            // Arrange
            var dto = new TestEntityEditDto { Id = 1, Name = "Updated Entity" };
            var entity = dto.Adapt<TestEntity>();

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TestEntity>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<TestEntity>(e => e.Name == "Updated Entity")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEntity_WhenEntityExists()
        {
            // Arrange
            var entityId = 1;
            var entity = new TestEntity { Id = entityId, Name = "Entity to Delete" };

            _mockRepository.Setup(r => r.Query(false)).Returns(new List<TestEntity> { entity }.AsQueryable().BuildMock());
            _mockRepository.Setup(r => r.DeleteAsync(entityId)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(entityId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(entityId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDeleteEntity_WhenEntityDoesNotExist()
        {
            // Arrange
            var entityId = 1;
            _mockRepository.Setup(r => r.Query(false)).Returns(Enumerable.Empty<TestEntity>().AsQueryable().BuildMock());

            // Act
            await _service.DeleteAsync(entityId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }


    }
}
