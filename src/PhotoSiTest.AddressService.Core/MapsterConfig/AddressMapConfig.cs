using Mapster;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSi.AddressService.Core.MapsterConfig
{
    [ExcludeFromCodeCoverage]
    public static class AddressMapConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Address, AddressDto>.NewConfig()
                .Map(dest => dest.FullAddress, src => $"{src.Street}, {src.City}, {src.Country}");


            TypeAdapterConfig<AddressDto, Address>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Street, src => src.Street)
                .Map(dest => dest.City, src => src.City)
                .Map(dest => dest.PostalCode, src => src.PostalCode)
                .Map(dest => dest.Country, src => src.Country);
        }
    }
    
}
