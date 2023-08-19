using CurrencyDataProvider.Data.EF;
using CurrencyDataProvider.Domain;
using Newtonsoft.Json;

namespace RatesCollector
{
    internal class RatesCollector
    {
        private readonly string _baseUri;
        private readonly string _apiKey;

        public RatesCollector(string baseUri, string apiKey)
        {
            _baseUri = baseUri;
            _apiKey = apiKey;
        }

        internal async Task CollectExchangeRates()
        {
            var url = GetFixerUrl();

            ExchangeRate exchangeRates;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                exchangeRates = DeserializeData(response.Content.ReadAsStringAsync().Result);
            }

            
            if (exchangeRates != null)
            {
                var currencyInformation = ConverData(exchangeRates);
                SaveData(currencyInformation);
            }
        }

        private string GetFixerUrl()
        {
            return $"{_baseUri}latest?access_key={_apiKey}";
        }

        private ExchangeRate DeserializeData(string data)
        {
            return JsonConvert.DeserializeObject<ExchangeRate>(data);
        }

        private void SaveData(CurrenciesInformation currenciesInformation)
        {
            using (var context = new CurrencyDataProviderDbContext())
            {
                context.CurrenciesInformations.Add(currenciesInformation);
                context.SaveChanges();
            }
        }

        private CurrenciesInformation ConverData(ExchangeRate data)
        {
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

            var currencyInfo = new CurrenciesInformation()
            {
                TimeStamp = int.Parse(data.TimeStamp),
                Date = DateTime.Parse(data.Date),
                Rates = rates
            };

            return currencyInfo;
        }
    }
}
