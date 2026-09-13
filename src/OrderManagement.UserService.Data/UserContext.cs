using Microsoft.EntityFrameworkCore;
using OrderManagement.Common.BaseClasses;
using OrderManagement.UserService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.UserService.Data
{
    [ExcludeFromCodeCoverage]
    //Add-Migration InitialMigration -OutputDir Migrations
    public class UserContext : BaseContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }  // Tabella degli utenti
    }
}
