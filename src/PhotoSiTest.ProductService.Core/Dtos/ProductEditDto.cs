using System.ComponentModel.DataAnnotations;

namespace PhotoSiTest.ProductService.Core.Dtos
{
    public class ProductEditDto
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int ProductCategoryId { get; set; } // Riferimento alla categoria tramite ID
        [Required]
        public string Description { get; set; }

    }
}
