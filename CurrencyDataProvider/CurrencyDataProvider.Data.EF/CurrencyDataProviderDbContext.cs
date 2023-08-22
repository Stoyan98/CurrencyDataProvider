using CurrencyDataProvider.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyDataProvider.Data.EF
{
    public class CurrencyDataProviderDbContext : DbContext
    {
        public DbSet<CurrenciesInformation> CurrenciesInformations { get; set; }
        public DbSet<Rate> Rates { get; set; }

        public DbSet<Request> Requests { get; set; }

        public CurrencyDataProviderDbContext(DbContextOptions<CurrencyDataProviderDbContext> options) : base(options)
        {
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
