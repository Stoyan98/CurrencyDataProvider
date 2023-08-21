using System;

namespace CurrencyDataProvider.Core.Request
{
    public record AddRequestCommand(
        string ServiceName, 
        string RequestId, 
        DateTime RequestDateUtc, 
        string ClientId);
}
