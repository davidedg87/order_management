using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.Common.Tests
{
    public class BaseRepositoryTests
    {
        private DbContextOptions<TestDbContext> CreateInMemoryOptions(string dbName)
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }



        [Fact]
        public async Task AddAsync_AddsEntityToDatabase()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(AddAsync_AddsEntityToDatabase));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity = new() { Id = 1, Name = "Test Entity" };

            // Act
            await repository.AddAsync(entity);
            await context.SaveChangesAsync();

            // Assert
            int entityCount = await context.TestEntities.CountAsync();
            entityCount.Should().Be(1);  // Verifica che ci sia una sola entità nel database
        }

        [Fact]
        public async Task Query_ReturnsCorrectEntityById()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(Query_ReturnsCorrectEntityById));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity = new() { Id = 1, Name = "Test Entity" };

            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            // Act
            TestEntity? result = await repository.Query().FirstOrDefaultAsync(e => e.Id == 1);

            // Assert
            result.Should().NotBeNull();  // Verifica che l'entità non sia nulla
            result.Id.Should().Be(entity.Id);  // Verifica che l'ID sia corretto
            result.Name.Should().Be(entity.Name);  // Verifica che il nome sia corretto
        }

        [Fact]
        public async Task Query_ReturnsAllEntities()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(Query_ReturnsAllEntities));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            List<TestEntity> entities = new()
            {
            new TestEntity { Id = 1, Name = "Entity 1" },
            new TestEntity { Id = 2, Name = "Entity 2" }
        };

            await context.TestEntities.AddRangeAsync(entities);
            await context.SaveChangesAsync();

            // Act
            List<TestEntity> result = await repository.Query().ToListAsync();

            // Assert
            result.Should().HaveCount(2);  // Verifica che ci siano due entità
            result.Select(e => e.Id).Should().Contain(new[] { 1, 2 });  // Verifica che gli ID siano corretti
        }

        [Fact]
        public async Task Query_WithTrack_ReturnsTrackedEntity()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(Query_WithTrack_ReturnsTrackedEntity));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity = new() { Id = 1, Name = "Test Entity" };

            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            // Act
            TestEntity? result = await repository.Query(track: true).FirstOrDefaultAsync(e => e.Id == 1);

            // Assert
            result.Should().NotBeNull();  // Verifica che l'entità non sia nulla
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TestEntity?> entry = context.Entry(result);  // Ottieni l'entry dell'entità nel contesto
            entry.State.Should().Be(EntityState.Unchanged);  // Verifica che l'entità sia tracciata come non modificata
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(UpdateAsync_UpdatesEntity));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity = new() { Id = 1, Name = "Old Name" };

            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            // Act
            entity.Name = "New Name";
            await repository.UpdateAsync(entity);
            await context.SaveChangesAsync();

            // Assert
            TestEntity? updated = await context.TestEntities.FindAsync(1);
            updated.Should().NotBeNull();  // Verifica che l'entità aggiornata non sia nulla
            updated.Name.Should().Be("New Name");  // Verifica che il nome sia stato aggiornato correttamente
        }




        [Fact]
        public async Task DeleteAsync_SoftDeletesEntity_WhenDeleted()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(DeleteAsync_SoftDeletesEntity_WhenDeleted));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity = new() { Id = 1, Name = "Test Entity" };

            // Aggiungi entità al contesto
            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            // Act - Soft delete
            await repository.DeleteAsync(entity.Id);
            await context.SaveChangesAsync();

            // Assert - Verifica che IsDeleted sia true e DeletedAt sia impostato
            // Disabilitiamo i global query filters per includere anche le entità soft deleted
            TestEntity? deletedEntity = await context.TestEntities
                                                       .IgnoreQueryFilters()  // Ignora i global query filters
                                                       .FirstOrDefaultAsync(e => e.Id == 1);

            deletedEntity.Should().NotBeNull();  // Verifica che l'entità non sia null
            deletedEntity.IsDeleted.Should().BeTrue();  // Verifica che l'entità sia marcata come eliminata
            deletedEntity.DeletedAt.Should().NotBeNull();  // Verifica che DeletedAt sia impostato
        }

        [Fact]
        public async Task Query_ReturnsOnlyNonDeletedEntities()
        {
            // Arrange
            DbContextOptions<TestDbContext> options = CreateInMemoryOptions(nameof(Query_ReturnsOnlyNonDeletedEntities));
            using TestDbContext context = new(options);
            BaseRepository<TestEntity> repository = new(context);
            TestEntity entity1 = new() { Id = 1, Name = "Entity 1" };
            TestEntity entity2 = new() { Id = 2, Name = "Entity 2", IsDeleted = true, DeletedAt = DateTimeOffset.UtcNow };

            // Aggiungi entità al contesto
            await context.TestEntities.AddRangeAsync(entity1, entity2);
            await context.SaveChangesAsync();

            // Act - Query delle entità non eliminate
            List<TestEntity> result = await repository.Query().Where(e => !e.IsDeleted).ToListAsync();

            // Assert - Verifica che solo le entità non eliminate siano restituite
            result.Should().ContainSingle();  // Deve contenere solo un'entità
            result.First().Id.Should().Be(1);  // L'entità con ID 1 deve essere restituita
        }

    
    }
}
