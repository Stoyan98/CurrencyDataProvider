using System;
using System.Collections.Generic;

namespace CurrencyDataProvider.Domain
{
    public sealed class CurrenciesInformation
    {
        public int Id { get; set; }

        public int TimeStamp { get; set; }

        public DateTime Date { get; set; }

        public List<Rate> Rates { get; set; } = new List<Rate>();
    }
}
