using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "attribute")]
    /*
        -- Attributes are used to store values for properties of entities.
        -- Attributes are mutable, their values may change over one or more types of time.
        -- Attributes have four flavors: static, historized, knotted static, and knotted historized.
    */
    public class Attribute
    {
        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }

        [XmlAttribute(AttributeName = "mnemonic")]
        public string Mnemonic { get; set; }

        [XmlAttribute(AttributeName = "descriptor")]
        public string Descriptor { get; set; }

        [XmlAttribute(AttributeName = "dataRange")]
        public string DataRange { get; set; }

        [XmlAttribute(AttributeName = "timeRange")]
        public string TimeRange { get; set; }

        [XmlAttribute(AttributeName = "knotRange")]
        public string KnotRange { get; set; }
    }
}