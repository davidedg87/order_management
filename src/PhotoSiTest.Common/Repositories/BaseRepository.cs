using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.Common.Interfaces;

namespace PhotoSiTest.Common.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual IQueryable<T> Query(bool track = false)
        {
            if(track)
                return _context.Set<T>().AsQueryable();
            else
                return _context.Set<T>().AsNoTracking().AsQueryable();


        }
    }
}
