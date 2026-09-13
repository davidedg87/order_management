using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderManagement.Common.BaseClasses;
using OrderManagement.Common.Interceptors;

namespace OrderManagement.Common.Tests
{
    public class TestDbContext : BaseContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; }

    }
}
