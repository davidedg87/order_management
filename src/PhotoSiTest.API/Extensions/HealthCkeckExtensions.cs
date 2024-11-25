using PhotoSi.AddressService.Data;
using PhotoSi.ProductService.Data;
using PhotoSiTest.OrderService.Data;
using PhotoSiTest.UserService.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class HealthCheckExtensions
    {
        public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
            .AddNpgSql(
                connectionString: configuration["ConnectionStrings:DefaultConnection"]!,
                name: "postgresql",
                tags: new[] { "db", "postgres" })
            .AddDbContextCheck<AddressContext>()
            .AddDbContextCheck<OrderContext>()
            .AddDbContextCheck<ProductContext>()
            .AddDbContextCheck<UserContext>();

        }
    }
}
