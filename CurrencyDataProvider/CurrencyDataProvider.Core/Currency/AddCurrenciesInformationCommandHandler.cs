using System.Threading.Tasks;
using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Core.Currency
{
    internal class AddCurrenciesInformationCommandHandler : ICommandHandler<AddCurrenciesInformationCommand>
    {
        public AddCurrenciesInformationCommandHandler(ICurrencyRepository currencyRepository)
        {
            CurrencyRepository = currencyRepository;
        }

        public ICurrencyRepository CurrencyRepository { get; set; }

        public async Task HandleAsync(AddCurrenciesInformationCommand command)
        {
            await CurrencyRepository.SaveCurrencyInfoAsync(
                new CurrenciesInformation
                {
                    TimeStamp = command.Timestamp,
                    Date = command.Date,
                    Rates = command.Rates
                });
        }
    }
}
