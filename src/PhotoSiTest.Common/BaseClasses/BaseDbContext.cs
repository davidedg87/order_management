using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotoSiTest.Common.BaseInterfaces;
using PhotoSiTest.Common.Extensions;
using PhotoSiTest.Common.Interceptors;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.Common.BaseClasses
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseContext : DbContext
    {
        protected BaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IInterceptor[] interceptors =
            {
                new SoftDeleteInterceptor()
            };

            // Aggiungi un interceptor comune per tutti i contesti
            optionsBuilder.AddInterceptors(interceptors);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region QueryFilters and Indexes

            modelBuilder.ApplyGlobalFiltersByInterface<ISoftDeletable>(e => !e.IsDeleted, nameof(ISoftDeletable.IsDeleted));

            #endregion
        }
    }
}
