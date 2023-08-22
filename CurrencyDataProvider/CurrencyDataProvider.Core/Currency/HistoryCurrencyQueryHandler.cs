using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Currency
{
    public class HistoryCurrencyQueryHandler : IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public HistoryCurrencyQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async ValueTask<List<HistoryCurrencyQueryResult>> HandleAsync(HistoryCurrencyQuery query)
        {
            int.TryParse(query.PeriodInHours, out int period);
            var dateNowUtc22 = DateTime.UtcNow.AddHours(-period);
            var resultDate = new DateTimeOffset(DateTime.UtcNow.AddHours(-period)).ToUnixTimeSeconds().ToString();
            int.TryParse(resultDate, out int fromTimeStamp);

            var historyRecords = await _currencyRepository.GetHistoryForCurrency(query.Currency, fromTimeStamp);

            var result = new List<HistoryCurrencyQueryResult>();

            foreach ( var historyRecord in historyRecords )
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(historyRecord.TimeStamp).DateTime;
                var rate = historyRecord.Rates.FirstOrDefault(x => x.Currency == query.Currency);
                result.Add(new HistoryCurrencyQueryResult(rate.Currency, rate.Amount, date));
            }

            return result;
        }
    }
}
