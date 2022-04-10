using Microsoft.EntityFrameworkCore;

namespace WebAppBV.Data
{
    public class BVContext : DbContext
    {
        public BVContext(DbContextOptions<BVContext> options) : base(options) { }

        public DbSet<Models.Transaction> Transactions { get; set; }
    }
}
