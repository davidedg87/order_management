using PhotoSi.AddressService.Data;
using Microsoft.EntityFrameworkCore;
using PhotoSi.ProductService.Data;
using PhotoSiTest.UserService.Data;
using PhotoSiTest.OrderService.Data;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DbContextExtensions
    {
        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura i contesti utilizzando il metodo generico
            AddDbContext<AddressContext>(services, configuration);
            AddDbContext<ProductContext>(services, configuration);
            AddDbContext<UserContext>(services, configuration);
            AddDbContext<OrderContext>(services, configuration);
        }

        private static void AddDbContext<TContext>(IServiceCollection services, IConfiguration configuration)
       where TContext : DbContext
        {
            services.AddDbContext<TContext>((serviceProvider, options) =>
                options.UseNpgsql(
                    configuration["ConnectionStrings:DefaultConnection"],
                    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null
                    )
                )
            );
        }

        public static void ApplyMigrations(this WebApplication app)
        {
            // Configura i contesti utilizzando il metodo generico
            app.ApplyMigrations<AddressContext>();
            app.ApplyMigrations<ProductContext>();
            app.ApplyMigrations<UserContext>();
            app.ApplyMigrations<OrderContext>();
        }


        public static void ApplyMigrations<TDbContext>(this WebApplication app) where TDbContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                dbContext.Database.Migrate(); // Applica tutte le migration pendenti
            }
        }

    }
}
