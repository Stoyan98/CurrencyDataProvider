namespace CurrencyDataProvider.JsonAPI.DataContracts
{
    public class HistoryRequest
    {
        public string RequestId { get; set; }

        public string TimeStamp { get; set; }

        public string Client { get; set; }

        public string Currency { get; set; }

        public string Period { get; set; }
    }
}
