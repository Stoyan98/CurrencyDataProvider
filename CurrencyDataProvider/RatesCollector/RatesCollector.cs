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

            ExchangeRate result;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                result = DeserializeData(response.Content.ReadAsStringAsync().Result);
            }

            //TODO: Save in Database here!!!
        }

        private string GetFixerUrl()
        {
            return $"{_baseUri}latest?access_key={_apiKey}";
        }

        private ExchangeRate DeserializeData(string data)
        {
            return JsonConvert.DeserializeObject<ExchangeRate>(data);
        }
    }
}
