using Microsoft.EntityFrameworkCore;
using OnlineConverter.Models;

namespace OnlineConverter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrentRate> CurrentRates { get; set; }
        public DbSet<UsdGraph> UsdGraphs { get; set; }
        public DbSet<EurGraph> EurGraphs { get; set; }
    }
}
