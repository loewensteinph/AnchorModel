using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "layout")]
    public class Layout
    {
        [XmlAttribute(AttributeName = "x")]
        public string X { get; set; }

        [XmlAttribute(AttributeName = "y")]
        public string Y { get; set; }

        [XmlAttribute(AttributeName = "fixed")]
        public string Fixed { get; set; }
    }
}