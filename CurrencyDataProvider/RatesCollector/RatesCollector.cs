using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RatesCollector
{
    public class RatesCollector : IRatesCollector
    {
        private readonly ILogger<RatesCollector> _log;
        private readonly IConfiguration _configuration;
        private readonly ICommandHandler<AddCurrenciesInformationCommand> _addCurrenciesInformationCommandHandler;

        public RatesCollector(
            ILogger<RatesCollector> log, 
            IConfiguration configuration,
            ICommandHandler<AddCurrenciesInformationCommand> addCurrenciesInformationCommandHandler)
        {
            _log = log;
            _configuration = configuration;
            _addCurrenciesInformationCommandHandler = addCurrenciesInformationCommandHandler;
        }

        public async Task CollectExchangeRates()
        {
            var url = GetFixerUrl();

            ExchangeRate exchangeRates;

            using (var client = new HttpClient())
            {
                _log.LogInformation($"Calling url {url}");

                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                _log.LogInformation($"Status code {response.StatusCode} from response of enpoint: {url}");

                _log.LogInformation("Response deserializing starting");

                exchangeRates = DeserializeData(response.Content.ReadAsStringAsync().Result);

                _log.LogInformation($"Response deserialized: {exchangeRates}");
            }

            if (exchangeRates != null)
            {
                var addCurrenciesInformationCommand = ConverData(exchangeRates);

                _log.LogInformation($"Invoking HandleAsync for AddCurrenciesInformationCommandHandler");

                await _addCurrenciesInformationCommandHandler.HandleAsync(addCurrenciesInformationCommand);

                _log.LogInformation("AddCurrenciesInformationCommandHandler saved in Database");
            }
        }

        private string GetFixerUrl()
        {
            var baseUri = _configuration.GetSection("BaseUri").Value;
            var apiKey = _configuration.GetSection("ApiKey").Value;

            return $"{baseUri}latest?access_key={apiKey}";
        }

        private ExchangeRate DeserializeData(string data)
        {
            return JsonConvert.DeserializeObject<ExchangeRate>(data);
        }

        private AddCurrenciesInformationCommand ConverData(ExchangeRate data)
        {
            _log.LogInformation("Converting ExchangeRate object to AddCurrenciesInformationCommand");

            var rates = new List<Rate>();

            foreach (var item in data.Rates)
            {
                decimal.TryParse(item.Value, out var amount);

                var rate = new Rate()
                {
                    Currency = item.Key,
                    Amount = amount
                };

                rates.Add(rate);
            }

            int.TryParse(data.TimeStamp, out int timestamp);
            DateTime.TryParse(data.Date, out DateTime date);

            return new AddCurrenciesInformationCommand(timestamp, date, rates);
        }
    }
}
