using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSi.ProductService.Data;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.ProductService.Data.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly DbContextOptions<ProductContext> _options;

        public ProductRepositoryTests()
        {
            // Configura in memoria il contesto del database
            _options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "ProductServiceTestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddProduct_ShouldSaveProductCorrectly()
        {
            var productRepository = new ProductRepository(new ProductContext(_options));

            // Crea un nuovo prodotto da salvare
            var product = new Product
            {
                Name = "Test Product",
                Price = 19.99m,
                ProductCategoryId = 1,
                Description = "A sample product for testing"
            };

            // Aggiungi il prodotto al repository
            await productRepository.AddAsync(product);

            // Verifica che il prodotto sia stato salvato correttamente
            using (var context = new ProductContext(_options))
            {
                var savedProduct = await context.Products.FindAsync(product.Id);

                // Asserzioni con FluentAssertions
                savedProduct.Should().NotBeNull(); // Verifica che il prodotto non sia null
                savedProduct.Name.Should().Be(product.Name); // Confronta nome
                savedProduct.Price.Should().Be(product.Price); // Confronta prezzo
                savedProduct.ProductCategoryId.Should().Be(product.ProductCategoryId); // Confronta categoria
                savedProduct.Description.Should().Be(product.Description); // Confronta descrizione
            }
        }
    }
}
