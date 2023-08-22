using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.Data;
using CurrencyDataProvider.Data.EF;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

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

builder.Services.AddDbContext<CurrencyDataProviderDbContext>();
//services.AddDbContext<CurrencyDataProviderDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IRequestRepository, RequestRepository>()
    .AddTransient<ICurrencyRepository, CurrencyRepository>();

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "XmlApi_";
});

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
