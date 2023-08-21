using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

using System.Net;

using CurrencyDataProvider.Data.EF;
using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.Data;
using System.Collections.Generic;

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
                .AddTransient<IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>>, HistoryCurrencyQueryHandler>();

            services.AddDbContext<CurrencyDataProviderDbContext>();
            //services.AddDbContext<CurrencyDataProviderDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IRequestRepository, RequestRepository>()
                .AddTransient<ICurrencyRepository, CurrencyRepository>();

            services.AddSwaggerGen();
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

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
