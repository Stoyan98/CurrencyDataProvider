using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Core.Currency
{
    public class AddCurrenciesInformationCommandHandler : ICommandHandler<AddCurrenciesInformationCommand>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public AddCurrenciesInformationCommandHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task HandleAsync(AddCurrenciesInformationCommand command)
        {
            await _currencyRepository.SaveCurrencyInfoAsync(
                new CurrenciesInformation
                {
                    TimeStamp = command.Timestamp,
                    Date = command.Date,
                    Rates = command.Rates
                });
        }
    }
}
