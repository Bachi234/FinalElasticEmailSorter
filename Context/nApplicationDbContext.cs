using automationTest.Models;
using Microsoft.EntityFrameworkCore;

namespace automationTest.Context
{
    public class ElasticDbContext : DbContext
    {
        public ElasticDbContext(DbContextOptions<ElasticDbContext> options) : base(options)
        {
        }

        public DbSet<tblElasticData> tblElasticData { get; set; }
    }

    public class MktgDbContext : DbContext
    {
        public MktgDbContext(DbContextOptions<MktgDbContext> options) : base(options)
        {
        }

        public DbSet<tblEvent> tblEvent { get; set; }
    }
}
