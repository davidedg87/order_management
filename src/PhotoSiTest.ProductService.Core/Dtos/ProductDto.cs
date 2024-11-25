namespace PhotoSiTest.ProductService.Core.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ProductCategoryId { get; set; } // Riferimento alla categoria tramite ID
        public string ProductCategoryName { get; set; } // Riferimento alla categoria tramite ID
        public string Description { get; set; }

    }
}
