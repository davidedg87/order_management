using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.UserService.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace PhotoSiTest.UserService.Data
{
    [ExcludeFromCodeCoverage]
    //Add-Migration InitialMigration -OutputDir Migrations
    public class UserContext : BaseContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }  // Tabella degli utenti
    }
}
