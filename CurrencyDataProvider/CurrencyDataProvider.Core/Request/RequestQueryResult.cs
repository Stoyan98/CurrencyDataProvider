using System;

namespace CurrencyDataProvider.Core.Request
{
    public record RequestQueryResult(
        bool IsExisting,
        string? ServiceName, 
        string? RequestId, 
        DateTime? RequestDateUtc, 
        string? ClientId);
}
