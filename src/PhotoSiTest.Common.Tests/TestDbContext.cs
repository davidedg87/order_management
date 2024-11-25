using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotoSiTest.Common.BaseClasses;
using PhotoSiTest.Common.Interceptors;

namespace PhotoSiTest.Common.Tests
{
    public class TestDbContext : BaseContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; }

    }
}
