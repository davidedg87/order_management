using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.OrderService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.OrderService.Data
{
    //Add-Migration InitialMigration -OutputDir Migrations
    [ExcludeFromCodeCoverage]
    public class OrderContext : BaseContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }  // Tabella degli ordini

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mappare l'OrderStatus come stringa nel database
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(
                    v => v.ToString(), // Conversione da enum a stringa
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)); // Conversione da stringa a enum
        }


    }
}
