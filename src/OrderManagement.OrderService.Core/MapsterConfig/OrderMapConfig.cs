using Mapster;
using OrderManagement.OrderService.Core.Dtos;
using OrderManagement.OrderService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.OrderService.Core.MapsterConfig
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
