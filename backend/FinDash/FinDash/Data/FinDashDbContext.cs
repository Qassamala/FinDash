namespace FinDash.Data
{
    using global::FinDash.Models;
    using Microsoft.EntityFrameworkCore;

    public class FinDashDbContext : DbContext
    {
        public FinDashDbContext(DbContextOptions<FinDashDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }

}
