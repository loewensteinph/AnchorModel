using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "knotRole")]
    public class KnotRole
    {
        [XmlIgnore] public Knot Knot;

        [XmlAttribute(AttributeName = "role")]
        public string Role { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
    }
}