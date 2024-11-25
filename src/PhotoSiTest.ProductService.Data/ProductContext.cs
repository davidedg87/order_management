using Microsoft.EntityFrameworkCore;
using PhotoSi.ProductService.Core.Models;
using PhotoSiTest.Common.BaseClasses;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSi.ProductService.Data
{
    [ExcludeFromCodeCoverage]
    //Add-Migration InitialMigration -OutputDir Migrations
    public class ProductContext : BaseContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }  // Tabella prodotti
        public DbSet<ProductCategory> ProductCategories { get; set; }  // Tabella categorie prodotti
    }
}