using System.Xml.Serialization;

namespace CurrencyDataProvider.XmlApi.DataContracts
{
    [XmlRoot(ElementName = "command")]
    public class HistoryRequest
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("history", Type = typeof(History))]
        public History History { get; set; }
    }

    public class History
    {
        [XmlAttribute(AttributeName = "consumer")]
        public string Consumer { get; set; }

        [XmlAttribute(AttributeName = "currency")]
        public string Currency { get; set; }

        [XmlAttribute(AttributeName = "period")]
        public string Period { get; set; }
    }
}
