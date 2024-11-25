
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace PhotoSi.ProductService.Core.Models
{
    [Index(nameof(Name), nameof(Description), IsUnique = true)]
    public class ProductCategory : BaseEntity
    {
        [Required]
        public string Name { get; set; }        // Nome della categoria
        [Required]
        public string Description { get; set; } // Descrizione della categoria
    }
}

