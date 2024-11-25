using Mapster;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.ProductService.Core.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.ProductService.Core.MapsterConfig
{
    [ExcludeFromCodeCoverage]
    public static class ProductMapConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Product, ProductDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id) // Mapping del campo base (da BaseEntity)
            .Map(dest => dest.ProductCategoryName, src => src.ProductCategory.Name); // Mapping del nome della categoria

            TypeAdapterConfig<ProductCategory, ProductCategoryDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id) // Mapping ID
            .Map(dest => dest.Name, src => src.Name) // Mapping Nome
            .Map(dest => dest.Description, src => src.Description); // Mapping Descrizione
        }
    }
}
