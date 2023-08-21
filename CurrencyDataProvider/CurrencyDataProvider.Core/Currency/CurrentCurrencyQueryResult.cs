namespace CurrencyDataProvider.Core.Currency
{
    public record CurrentCurrencyQueryResult(
        int Timestamp, 
        string Currency, 
        decimal Amount);
}
