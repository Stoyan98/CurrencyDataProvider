using System;
using System.Collections.Generic;

namespace CurrencyDataProvider.Domain
{
    public class ExchangeRate
    {
        public string? Success { get; set; }

        public string? TimeStamp { get; set; }

        public string? Base { get; set; }

        public string? Date { get; set; }

        public Dictionary<string, string> Rates { get; set; }
    }
}
