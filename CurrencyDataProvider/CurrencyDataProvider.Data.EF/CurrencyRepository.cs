﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyDataProvider.Domain;
using Microsoft.EntityFrameworkCore;

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

        public async Task<CurrenciesInformation> GetLatestCurrencyRateAsync(string currency)
        {
            var result = await DbContext.CurrenciesInformations
                                        .Include(x => x.Rates
                                                       .Where(r=>r.Currency == currency))
                                        .OrderBy(x=>x.Id)
                                        .LastOrDefaultAsync();

            return result;
        }

        public async Task<List<CurrenciesInformation>> GetHistoryForCurrency(string currency, int fromTimeStamp)
        {
            var result = await DbContext.CurrenciesInformations
                                        .Include(x=>x.Rates.Where(r=>r.Currency == currency))
                                        .Where(x=>x.TimeStamp > fromTimeStamp)
                                        .OrderBy(x=>x.TimeStamp)
                                        .ToListAsync();

            return result;
        }
    }
}
