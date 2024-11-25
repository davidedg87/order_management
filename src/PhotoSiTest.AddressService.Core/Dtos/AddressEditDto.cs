using System.ComponentModel.DataAnnotations;

namespace PhotoSi.AddressService.Core.Dtos
{
    public class AddressEditDto
    {
        public int? Id { get; set; }                // Id univoco dell'indirizzo
        [Required]
        public string Street { get; set; }         // Via dell'indirizzo
        [Required]
        public string City { get; set; }           // Città
        [Required]
        public string PostalCode { get; set; }     // Codice postale
        [Required]
        public string Country { get; set; }        // Paese
    }
}
