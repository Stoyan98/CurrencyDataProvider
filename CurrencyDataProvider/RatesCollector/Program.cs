using Microsoft.Extensions.Configuration;

namespace RatesCollector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

                IConfiguration config = builder.Build();

                var baseUri = config.GetSection("BaseUri");
                var apiKey = config.GetSection("ApiKey");

                new RatesCollector(baseUri.Value, apiKey.Value).CollectExchangeRates();
            }
            catch (Exception)
            {
                Console.WriteLine("Finishing program");
                throw;
            }
        }
    }
}