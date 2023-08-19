namespace CurrencyDataProvider.Domain
{
    public sealed class Rate
    {
        public int Id { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public int CurrenciesInformationId { get; set; }

        public CurrenciesInformation CurrenciesInformation { get; set; }
    }
}
