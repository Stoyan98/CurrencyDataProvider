using System.Threading.Tasks;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data.EF
{
    public class CurrencyRepository : ICurrencyRepository
    {
        public CurrencyRepository(CurrencyDataProviderDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public CurrencyDataProviderDbContext DbContext { get; set; }

        public async Task SaveCurrencyInfoAsync(CurrenciesInformation currenciesInformation)
        {
            await DbContext.AddAsync(currenciesInformation);
            await DbContext.SaveChangesAsync();
        }
    }
}
