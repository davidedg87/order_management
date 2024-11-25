using Mapster;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSiTest.OrderService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSi.OrderService.Core.MapsterConfig
{
    [ExcludeFromCodeCoverage]
    public static class OrderMapConfig
    { 
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Order, OrderDto>.NewConfig();
        }
    }
    
}
