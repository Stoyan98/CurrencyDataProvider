using System.Xml.Serialization;

namespace CurrencyDataProvider.XmlApi.DataContracts
{
    [XmlRoot(ElementName = "history")]
    public class HistoryResponse
    {
        [XmlElement("currency")]
        public string Currency { get; set; }

        [XmlElement("amount")]
        public decimal Amount { get; set; }

        [XmlElement("date")]
        public DateTime Date { get; set; }
    }
}
