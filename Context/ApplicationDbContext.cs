using automationTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace automationTest.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<tblElasticData> tblElasticData { get; set; }
        public DbSet<tblEvent> tblEvent { get; set; }
    }
}

