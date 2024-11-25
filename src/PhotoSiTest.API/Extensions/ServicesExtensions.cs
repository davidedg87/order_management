using PhotoSiTest.AddressService.Core;
using PhotoSiTest.AddressService.Data;
using PhotoSiTest.Common;
using PhotoSiTest.OrderService.Core;
using PhotoSiTest.OrderService.Data;
using PhotoSiTest.ProductService.Core;
using PhotoSiTest.ProductService.Data;
using PhotoSiTest.UserService.Core;
using PhotoSiTest.UserService.Data;
using Scrutor;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PhotoSiTest.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServicesExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {


            /*
                FromAssemblies - allows you to specify which assemblies to scan
                AddClasses - adds the classes from the selected assemblies
                UsingRegistrationStrategy - defines which RegistrationStrategy to use
                AsMatchingInterface - registers the types as matching interfaces (ClassName → IClassName)
                WithScopedLifetime - registers the types with a scoped service lifetime
                There are three values for RegistrationStrategy you can use:

                RegistrationStrategy.Skip - skips registrations if service already exists
                RegistrationStrategy.Append- appends a new registration for existing services
                RegistrationStrategy.Throw- throws when trying to register an existing service
             
             */


            //SCAN ASSEMBLY E REGISTRAZIONE AUTOMATICA DEI SERVIZI CON SCRUTOR
            services.Scan(selector =>
             selector
             .FromAssemblies(GetAssemblies())
             .AddClasses(publicOnly: false)
             .UsingRegistrationStrategy(RegistrationStrategy.Skip)
             .AsMatchingInterface()
             .WithScopedLifetime()
             );

        }

        public static Assembly[] GetAssemblies()
        {
            return new[]
            {
            typeof(MarkerAddressCore).Assembly,
            typeof(MarkerAddressData).Assembly,
            typeof(MarkerProductCore).Assembly,
            typeof(MarkerProductData).Assembly,
            typeof(MarkerUserCore).Assembly,
            typeof(MarkerUserData).Assembly,
            typeof(MarkerOrderCore).Assembly,
            typeof(MarkerOrderData).Assembly,
            typeof(MarkerCommon).Assembly
        };
        }


    }
}
