using PhotoSiTest.Common.BaseClasses;

namespace PhotoSiTest.Common.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query(bool track = false);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
