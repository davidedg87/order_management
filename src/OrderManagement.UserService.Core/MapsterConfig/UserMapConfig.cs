using Mapster;
using OrderManagement.UserService.Core.Dtos;
using OrderManagement.UserService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.UserService.Core.MapsterConfig
{
    [ExcludeFromCodeCoverage]
    public static class UserMapConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<User, UserDto>.NewConfig();
        }
    }
    
}
