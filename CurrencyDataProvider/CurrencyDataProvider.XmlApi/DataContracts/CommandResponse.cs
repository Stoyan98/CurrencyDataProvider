using System.Xml.Serialization;

namespace CurrencyDataProvider.XmlApi.DataContracts
{
    [XmlRoot(ElementName = "current_response")]
    public class CommandResponse
    {
        [XmlElement("timestamp")]
        public int TimeStamp { get; set; }

        [XmlElement("currency")]
        public string Currency { get; set; }

        [XmlElement("amount")]
        public decimal Amount{ get; set; }
    }
}
