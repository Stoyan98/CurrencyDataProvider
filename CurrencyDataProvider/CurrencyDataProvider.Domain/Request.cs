using System;

namespace CurrencyDataProvider.Domain
{
    public sealed class Request
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }

        public string RequestId { get; set; }

        public DateTime RequestDateUtc { get; set; }

        public string ClientId { get; set; }
    }
}
