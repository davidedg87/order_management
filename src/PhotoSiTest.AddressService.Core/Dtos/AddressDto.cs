namespace PhotoSi.AddressService.Core.Dtos
{
    public class AddressDto
    {
        public int Id { get; set; }                // Id univoco dell'indirizzo
        public string Street { get; set; }         // Via dell'indirizzo
        public string City { get; set; }           // Città
        public string PostalCode { get; set; }     // Codice postale
        public string Country { get; set; }        // Paese
        public string FullAddress { get; set; }    // Indirizzo completo (via, città, paese)
    }
}
