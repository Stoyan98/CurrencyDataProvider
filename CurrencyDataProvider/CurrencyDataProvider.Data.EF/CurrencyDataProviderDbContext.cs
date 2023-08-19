using CurrencyDataProvider.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyDataProvider.Data.EF
{
    public class CurrencyDataProviderDbContext : DbContext
    {
        public DbSet<CurrenciesInformation> CurrenciesInformations { get; set; }
        public DbSet<Rate> Rates { get; set; }

        public CurrencyDataProviderDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb; Database=CurrencyDb; Trusted_Connection=True; MultipleActiveResultSets=true");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrenciesInformation>()
                .HasMany(r => r.Rates)
                .WithOne(c => c.CurrenciesInformation)
                .HasForeignKey(c => c.CurrenciesInformationId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 6);
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
