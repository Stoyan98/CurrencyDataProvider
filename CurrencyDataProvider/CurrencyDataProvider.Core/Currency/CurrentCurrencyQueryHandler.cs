using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyDataProvider.Core.Currency
{
    public class CurrentCurrencyQueryHandler : IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult>
    {
        public CurrentCurrencyQueryHandler(ICurrencyRepository currencyRepository)
        {
            CurrencyRepository = currencyRepository;
        }

        public ICurrencyRepository CurrencyRepository { get; set; }

        public async ValueTask<CurrentCurrencyQueryResult> HandleAsync(CurrentCurrencyQuery query)
        {
            var latestRate = await CurrencyRepository.GetLatestCurrencyRateAsync(query.Currency);

            return new CurrentCurrencyQueryResult(
                latestRate.TimeStamp,
                latestRate.Rates.FirstOrDefault().Currency,
                latestRate.Rates.FirstOrDefault().Amount);
        }
    }
}
