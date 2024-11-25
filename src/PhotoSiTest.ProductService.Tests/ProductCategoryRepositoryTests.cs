using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSi.ProductService.Data;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.ProductService.Data.Repositories
{
    public class ProductCategoryRepositoryTests
    {
        private readonly DbContextOptions<ProductContext> _options;

        public ProductCategoryRepositoryTests()
        {
            // Configura in memoria il contesto del database
            _options = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(databaseName: "ProductServiceTestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddProductCategory_ShouldSaveProductCategoryCorrectly()
        {
            var productCategoryRepository = new ProductCategoryRepository(new ProductContext(_options));

            // Crea una nuova categoria di prodotto da salvare
            var productCategory = new ProductCategory
            {
                Name = "Electronics",
                Description = "Category for electronic products"
            };

            // Aggiungi la categoria di prodotto al repository
            await productCategoryRepository.AddAsync(productCategory);

            // Verifica che la categoria di prodotto sia stata salvata correttamente
            using (var context = new ProductContext(_options))
            {
                var savedProductCategory = await context.ProductCategories.FindAsync(productCategory.Id);

                // Asserzioni con FluentAssertions
                savedProductCategory.Should().NotBeNull(); // Verifica che la categoria di prodotto non sia null
                savedProductCategory.Name.Should().Be(productCategory.Name); // Confronta nome
                savedProductCategory.Description.Should().Be(productCategory.Description); // Confronta descrizione
            }
        }
    }
}
