using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Data.EF;
using CurrencyDataProvider.Core.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddControllersWithViews()
    .AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddTransient<ICommandHandler<AddCurrenciesInformationCommand>, AddCurrenciesInformationCommandHandler>()
                .AddTransient<ICommandHandler<AddRequestCommand>, AddRequestCommandHandler>()
                .AddTransient<IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult>, CurrentCurrencyQueryHandler>()
                .AddTransient<IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>>, HistoryCurrencyQueryHandler>()
                .AddTransient<IQueryHandler<RequestQuery, RequestQueryResult>, RequestQueryHandler>();

builder.Services.AddDbContext<CurrencyDataProviderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IRequestRepository, RequestRepository>()
    .AddTransient<ICurrencyRepository, CurrencyRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "XmlApi_";
});

var configSection = builder.Configuration.GetSection("RabbitMQSettings");
var settings = new RabbitMQSettings();
configSection.Bind(settings);

builder.Services.AddSingleton<RabbitMQSettings>(settings);
builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
{
    HostName = settings.HostName
});
builder.Services.AddSingleton<ModelFactory>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<ModelFactory>().CreateChannel());
builder.Services.AddSingleton<RabbitSender>();

var app = builder.Build();

app.MapHealthChecks("/health");

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/xml";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            //TODO: Logging logic goes here!!!
            await context.Response.WriteAsync(context.Response.StatusCode + " Internal Server Error");
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
