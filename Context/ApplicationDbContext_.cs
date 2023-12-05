using automationTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace automationTest.Context
{
    public class ApplicationDbContext_ : DbContext
    {
        public ApplicationDbContext_(DbContextOptions<ApplicationDbContext_> options) : base(options)
        {
        }
        public DbSet<tblEvent> tblEvent { get; set; }
    }
}


