using Microsoft.EntityFrameworkCore;
using OrderManagement.AddressService.Core.Models;
using OrderManagement.Common.BaseClasses;
using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.AddressService.Data
{
    [ExcludeFromCodeCoverage]
    //Add-Migration InitialMigration -OutputDir Migrations
    public class AddressContext : BaseContext
    {
        public AddressContext(DbContextOptions<AddressContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }  // Tabella degli indirizzi
    }
}