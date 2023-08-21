using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyDataProvider.Core.Currency
{
    public class HistoryCurrencyQueryHandler : IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>>
    {
        public HistoryCurrencyQueryHandler(ICurrencyRepository currencyRepository)
        {
            CurrencyRepository = currencyRepository;
        }

        public ICurrencyRepository CurrencyRepository { get; set; }

        public async ValueTask<List<HistoryCurrencyQueryResult>> HandleAsync(HistoryCurrencyQuery query)
        {
            int.TryParse(query.periodInHours, out int period);
            var dateNowUtc22 = DateTime.UtcNow.AddHours(-period);
            var resultDate = new DateTimeOffset(DateTime.UtcNow.AddHours(-period)).ToUnixTimeSeconds().ToString();
            int.TryParse(resultDate, out int fromTimeStamp);

            var historyRecords = await CurrencyRepository.GetHistoryForCurrency(query.currency, fromTimeStamp);

            var result = new List<HistoryCurrencyQueryResult>();

            foreach ( var historyRecord in historyRecords )
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(historyRecord.TimeStamp).DateTime;
                var rate = historyRecord.Rates.FirstOrDefault(x => x.Currency == query.currency);
                result.Add(new HistoryCurrencyQueryResult(rate.Currency, rate.Amount, date));
            }

            return result;
        }
    }
}
