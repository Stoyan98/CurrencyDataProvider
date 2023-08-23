using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.File;

namespace RatesCollector
{
    internal class Program
    {
        private static readonly string LogFilePath = "D:\\Projects\\CurrencyDataProvider\\CurrencyDataProvider\\RatesCollector\\log.txt";


        static void Main(string[] args)
        {
            var host = AppStartup();

            try
            {
                var ratesCollector = ActivatorUtilities.CreateInstance<RatesCollector>(host.Services);

                ratesCollector.CollectExchangeRates().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Something went wrong {ex}");
                throw;
            }

            Log.Logger.Information("Application Ended");
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Build())
                            .Enrich.FromLogContext()
                            .WriteTo.File(LogFilePath)
                            .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder()
                        .ConfigureServices((context, services) => {
                            services.AddDbContext<CurrencyDataProviderDbContext>(options => 
                                                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                            services.AddTransient<ICurrencyRepository, CurrencyRepository>();

                            services.AddTransient<IRatesCollector, RatesCollector>()
                                    .AddTransient<ICommandHandler<AddCurrenciesInformationCommand>, AddCurrenciesInformationCommandHandler>();
                        })
                        .UseSerilog()
                        .Build();

            return host;
        }
    }
}