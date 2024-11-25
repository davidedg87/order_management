using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.Common.Interfaces;

namespace PhotoSiTest.Common.Services
{
    public class BaseService<TEntity, TReadDto, TWriteDto> : IBaseService<TEntity, TReadDto, TWriteDto>
        where TEntity : BaseEntity
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly ILogger<BaseService<TEntity, TReadDto, TWriteDto>> _logger;

        public BaseService(IBaseRepository<TEntity> repository, ILogger<BaseService<TEntity, TReadDto, TWriteDto>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public virtual async Task<TReadDto?> GetByIdAsync(int id)
        {
            _logger.LogTrace("Fetching entity of type {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

            var result = await _repository.Query()
                                          .Where(entity => entity.Id == id)
                                          .Select(entity => entity.Adapt<TReadDto>())
                                          .FirstOrDefaultAsync();

            if (result != null)
            {
                _logger.LogTrace("Found entity of type {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
            }
            else
            {
                _logger.LogTrace("Entity of type {EntityType} with ID: {Id} not found", typeof(TEntity).Name, id);
            }

            return result;
        }

        public virtual async Task<IEnumerable<TReadDto>> GetAllAsync()
        {
            _logger.LogTrace("Fetching all entities of type {EntityType}", typeof(TEntity).Name);

            var result = await _repository.Query()
                                          .Select(entity => entity.Adapt<TReadDto>())
                                          .ToListAsync();

            _logger.LogTrace("Fetched {Count} entities of type {EntityType}", result.Count, typeof(TEntity).Name);

            return result;
        }

        public virtual async Task<PaginatedResult<TReadDto>> PaginateAsync(PagedFilter filter)
        {
            if (filter.PageNumber <= 0)
                throw new ArgumentException("PageNumber must be greater than zero.", nameof(filter.PageNumber));

            if (filter.PageSize <= 0)
                throw new ArgumentException("PageSize must be greater than zero.", nameof(filter.PageSize));

            // Esegui la query sul repository
            var query = _repository.Query();

            // Conta il totale degli elementi
            var totalCount = await query.CountAsync();

            // Recupera gli elementi paginati
            var items = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .Select(entity => entity.Adapt<TReadDto>())
                                   .ToListAsync();

            // Costruisci il risultato
            return new PaginatedResult<TReadDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public virtual async Task<int> CreateAsync(TWriteDto dto)
        {
            _logger.LogTrace("Creating new entity of type {EntityType} from DTO", typeof(TEntity).Name);

            var entity = dto.Adapt<TEntity>();
            await _repository.AddAsync(entity);

            _logger.LogTrace("Created new entity of type {EntityType} with Id {EntityId}", typeof(TEntity).Name, entity.Id);

            return entity.Id;
        }

        public virtual async Task UpdateAsync(TWriteDto dto)
        {
            _logger.LogTrace("Updating entity of type {EntityType} from DTO", typeof(TEntity).Name);

            var entity = dto.Adapt<TEntity>();
            await _repository.UpdateAsync(entity);

            _logger.LogTrace("Updated entity of type {EntityType}", typeof(TEntity).Name);
        }

        public virtual async Task DeleteAsync(int id)
        {
            _logger.LogTrace("Deleting entity of type {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

            var entity = await _repository.Query()
                                          .FirstOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                await _repository.DeleteAsync(entity.Id);
                _logger.LogTrace("Deleted entity of type {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
            }
            else
            {
                _logger.LogTrace("Entity of type {EntityType} with ID: {Id} not found for deletion", typeof(TEntity).Name, id);
            }
        }
    }
}
