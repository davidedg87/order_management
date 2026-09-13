using System.ComponentModel.DataAnnotations;

namespace OrderManagement.ProductService.Core.Dtos
{
    public class ProductCategoryEditDto
    {
        public int? Id { get; set; }             // Identificativo univoco della categoria
        [Required]
        public string Name { get; set; }        // Nome della categoria
        [Required]
        public string Description { get; set; } // Descrizione della categoria

    }
}
