namespace CurrencyDataProvider.JsonAPI.DataContracts
{
    public interface IRequest
    {
        public string RequestId { get; set; }

        public string Client { get; set; }
    }
}
