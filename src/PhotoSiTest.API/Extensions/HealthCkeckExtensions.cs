using PhotoSi.AddressService.Data;
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
            .AddDbContextCheck<AddressContext>();
        }
    }
}
