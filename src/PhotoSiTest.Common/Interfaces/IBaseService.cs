using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.Common.BaseTypes;

namespace PhotoSiTest.Common.Interfaces
{
    public interface IBaseService<TEntity, TReadDto, TWriteDto>
        where TEntity : BaseEntity
    {
        Task<TReadDto?> GetByIdAsync(int id);   // Ottieni un'entità tramite ID
        Task<IEnumerable<TReadDto>> GetAllAsync();            // Ottieni tutte le entità
        Task<PaginatedResult<TReadDto>> PaginateAsync(PagedFilter filter);

        Task<int> CreateAsync(TWriteDto dto);                       // Crea una nuova entità
        Task UpdateAsync(TWriteDto dto);                       // Modifica un'entità esistente
        Task DeleteAsync(int id);                         // Elimina un'entità
    }
}
