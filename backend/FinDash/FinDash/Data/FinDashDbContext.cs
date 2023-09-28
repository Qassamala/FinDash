using FinDash.Models;
using Microsoft.EntityFrameworkCore;


namespace FinDash.Data
{

    public class FinDashDbContext : DbContext
    {
        public FinDashDbContext(DbContextOptions<FinDashDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }


       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setting username as unique and applying indexing on username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Will enable later for added security to mitigate IDOR
            /*  modelBuilder.Entity<User>()
                  .Property(u => u.Id)
                  .ValueGeneratedNever(); */
        }

    }

}
