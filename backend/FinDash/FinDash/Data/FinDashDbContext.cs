using FinDash.Models;
using Microsoft.EntityFrameworkCore;


namespace FinDash.Data
{

    public class FinDashDbContext : DbContext
    {
        public FinDashDbContext(DbContextOptions<FinDashDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<StaticStockData> StaticStockData { get; set; }
        public DbSet<StockPrice> StockPrices { get; set; }



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

            modelBuilder.Entity<StaticStockData>()
                .HasIndex(s => s.Symbol)
                .IsUnique();

            modelBuilder.Entity<StockPrice>()
                .HasOne(sp => sp.StaticStockData)
                .WithMany(s => s.StockPrices)
                .HasForeignKey(sp => sp.StaticStockDataId);
        }

    }

}
