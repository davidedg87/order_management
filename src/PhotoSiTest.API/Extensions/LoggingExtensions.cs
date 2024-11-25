using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class LoggingExtensions
    {
        public static void AddLogging(this IHostBuilder hostBuilder, IConfiguration configurationSettings)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(configurationSettings);
            });

        }
    }
}
