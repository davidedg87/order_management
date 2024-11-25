
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace PhotoSi.ProductService.Core.Models
{
    [Index(nameof(Name), nameof(ProductCategoryId), IsUnique = true)]
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int ProductCategoryId { get; set; } // Riferimento alla categoria tramite ID
        public ProductCategory ProductCategory { get; set; } // Relazione diretta con la categoria
        [Required]
        public string Description { get; set; }
    }
}

