using System.Threading.Tasks;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data
{
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Save currencies information
        /// </summary>
        /// <param name="currenciesInformation"></param>
        /// <returns></returns>
        Task SaveCurrencyInfoAsync(CurrenciesInformation currenciesInformation);
    }
}
