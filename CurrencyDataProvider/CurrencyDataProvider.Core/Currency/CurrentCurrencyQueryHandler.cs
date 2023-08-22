using System.Linq;
using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Currency
{
    public class CurrentCurrencyQueryHandler : IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrentCurrencyQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async ValueTask<CurrentCurrencyQueryResult> HandleAsync(CurrentCurrencyQuery query)
        {
            var latestRate = await _currencyRepository.GetLatestCurrencyRateAsync(query.Currency);

            return new CurrentCurrencyQueryResult(
                latestRate.TimeStamp,
                latestRate.Rates.FirstOrDefault().Currency,
                latestRate.Rates.FirstOrDefault().Amount);
        }
    }
}
