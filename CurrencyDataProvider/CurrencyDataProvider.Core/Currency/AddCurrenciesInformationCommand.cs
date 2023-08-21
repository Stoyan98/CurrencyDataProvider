using System;
using System.Collections.Generic;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Core.Currency
{
    public record AddCurrenciesInformationCommand(
        int Timestamp, 
        DateTime Date, 
        List<Rate> Rates);
}
