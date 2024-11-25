using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace PhotoSiTest.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ModelExtensions
    {
        /*Riepilogo:
            Il metodo scorre tutte le entità del modello e verifica se implementano l'interfaccia TInterface.
            Se l'entità implementa l'interfaccia, crea una nuova versione del filtro (una Expression<Func<TEntity, bool>>) che può essere applicata a quell'entità concreta, adattando l'espressione iniziale.
            Applica questo filtro globale usando HasQueryFilter su ciascuna entità che implementa l'interfaccia.
            */

        public static void ApplyGlobalFiltersByInterface<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression, string indexPropertyName = "IsDeleted")
        {

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetInterface(typeof(TInterface).Name) != null)
                {
                    // Aggiungi il filtro globale
                    ParameterExpression newParam = Expression.Parameter(entityType.ClrType);
                    Expression newBody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(newBody, newParam));

                    // Aggiungi l'indice sulla proprietà specificata (di default "IsDeleted")
                    var foundProperty = entityType.FindProperty(indexPropertyName);
                    if (foundProperty != null)
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .HasIndex(foundProperty.Name); // Aggiungi indice sulla proprietà trovata
                    }

                }
            }

        }

    }
}
