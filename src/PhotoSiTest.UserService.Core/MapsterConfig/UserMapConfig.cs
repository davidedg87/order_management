using Mapster;
using PhotoSi.UserService.Core.Dtos;
using PhotoSiTest.UserService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSi.UserService.Core.MapsterConfig
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
