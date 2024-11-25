
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace PhotoSi.AddressService.Core.Models
{
    [Index(nameof(Street), nameof(City), nameof(PostalCode), nameof(Country), IsUnique = true)]
    public class Address : BaseEntity
    {
        [Required]
        public string? Street { get; set; }   // Via dell'indirizzo
        [Required]
        public string? City { get; set; }     // Città
        [Required]
        public string? PostalCode { get; set; } // Codice postale
        [Required]
        public string? Country { get; set; }  // Paese


    }
}

