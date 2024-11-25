using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotoSiTest.Common.BaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSiTest.Common.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {

            if (eventData.Context is null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            //Recupero tutte le entita' che implementano l'interfaccia ISoftDeletable e sono in stato Deleted
            IEnumerable<EntityEntry<ISoftDeletable>> entries =
               eventData
                   .Context
                   .ChangeTracker
                   .Entries<ISoftDeletable>()
                   .Where(e => e.State == EntityState.Deleted);

            //Per ogni entità trovata imposto lo stato a Modified e setto il flag IsDeleted a true
            foreach (EntityEntry<ISoftDeletable> softDeletable in entries)
            {
                softDeletable.State = EntityState.Modified;
                softDeletable.Entity.IsDeleted = true;
                softDeletable.Entity.DeletedAt = DateTime.UtcNow;
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        }

    }
}
