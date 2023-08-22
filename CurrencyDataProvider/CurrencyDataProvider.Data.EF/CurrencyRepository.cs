using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data.EF
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyDataProviderDbContext _dbContext;

        public CurrencyRepository(CurrencyDataProviderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveCurrencyInfoAsync(CurrenciesInformation currenciesInformation)
        {
            await _dbContext.AddAsync(currenciesInformation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CurrenciesInformation> GetLatestCurrencyRateAsync(string currency)
        {
            return await _dbContext.CurrenciesInformations
                                        .Include(x => x.Rates
                                                       .Where(r=>r.Currency == currency))
                                        .OrderBy(x=>x.Id)
                                        .LastOrDefaultAsync();
        }

        public async Task<List<CurrenciesInformation>> GetHistoryForCurrency(string currency, int fromTimeStamp)
        {
            return await _dbContext.CurrenciesInformations
                                        .Include(x=>x.Rates.Where(r=>r.Currency == currency))
                                        .Where(x=>x.TimeStamp > fromTimeStamp)
                                        .OrderBy(x=>x.TimeStamp)
                                        .ToListAsync();
        }
    }
}
