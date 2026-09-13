using OrderManagement.AddressService.Data;
using OrderManagement.ProductService.Data;
using OrderManagement.OrderService.Data;
using OrderManagement.UserService.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.API.Extensions
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
