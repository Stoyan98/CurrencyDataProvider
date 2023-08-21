using System;

namespace CurrencyDataProvider.Core.Currency
{
    public record HistoryCurrencyQueryResult(string currency, decimal amount, DateTime date);
}
