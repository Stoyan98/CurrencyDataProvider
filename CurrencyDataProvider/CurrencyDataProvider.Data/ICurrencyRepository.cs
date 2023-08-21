using System.Threading.Tasks;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data
{
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Saves currencies information
        /// </summary>
        /// <param name="currenciesInformation"></param>
        /// <returns></returns>
        Task SaveCurrencyInfoAsync(CurrenciesInformation currenciesInformation);

        /// <summary>
        /// Gets latest record for currency information
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        Task<CurrenciesInformation> GetLatestCurrencyRateAsync(string currency);
    }
}
