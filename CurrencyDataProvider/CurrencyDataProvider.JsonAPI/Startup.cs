using System.Net;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using CurrencyDataProvider.Data.EF;
using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Core.RabbitMQ;
using RabbitMQ.Client;

namespace CurrencyDataProvider.JsonAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHealthChecks();

            services.AddTransient<ICommandHandler<AddCurrenciesInformationCommand>, AddCurrenciesInformationCommandHandler>()
                .AddTransient<ICommandHandler<AddRequestCommand>, AddRequestCommandHandler>()
                .AddTransient<IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult>, CurrentCurrencyQueryHandler>()
                .AddTransient<IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>>, HistoryCurrencyQueryHandler>()
                .AddTransient<IQueryHandler<RequestQuery, RequestQueryResult>, RequestQueryHandler>();

            services.AddDbContext<CurrencyDataProviderDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IRequestRepository, RequestRepository>()
                .AddTransient<ICurrencyRepository, CurrencyRepository>();

            services.AddSwaggerGen();

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "JsonApi_";
            });

            SetUpRabbitMq(services, Configuration);
        }

        public void SetUpRabbitMq(IServiceCollection services, IConfiguration config)
        {
            var configSection = config.GetSection("RabbitMQSettings");
            var settings = new RabbitMQSettings();
            configSection.Bind(settings);
            // add the settings for later use by other classes via injection
            services.AddSingleton<RabbitMQSettings>(settings);

            // As the connection factory is disposable, need to ensure container disposes of it when finished
            services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
            {
                HostName = settings.HostName
            });

            services.AddSingleton<ModelFactory>();
            services.AddSingleton(sp => sp.GetRequiredService<ModelFactory>().CreateChannel());
            services.AddSingleton<RabbitSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health");

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //TODO: Logging logic goes here!!!
                        await context.Response.WriteAsync(context.Response.StatusCode + " Internal Server Error");
                    }
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
