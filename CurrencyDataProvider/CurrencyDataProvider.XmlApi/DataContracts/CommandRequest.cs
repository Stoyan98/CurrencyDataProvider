using System.Xml.Serialization;

namespace CurrencyDataProvider.XmlApi.DataContracts
{
    [XmlRoot(ElementName = "command")]
    public class CommandRequest
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("get", Type = typeof(GetData))]
        public List<GetData> Get { get; set; }
    }

    public class GetData
    {
        [XmlAttribute("consumer")]
        public string Consumer { get; set; }

        [XmlElement("currency")]
        public string Currency { get; set; }
    }
}
