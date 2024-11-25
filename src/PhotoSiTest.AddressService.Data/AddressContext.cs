using Microsoft.EntityFrameworkCore;
using PhotoSi.AddressService.Core.Models;
using PhotoSiTest.Common.BaseClasses;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSi.AddressService.Data
{
    [ExcludeFromCodeCoverage]
    //Add-Migration InitialMigration -OutputDir Migrations
    public class AddressContext : BaseContext
    {
        public AddressContext(DbContextOptions<AddressContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }  // Tabella degli indirizzi
    }
}