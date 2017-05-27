using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "anchorRole")]
    public class AnchorRole
    {
        [XmlIgnore] public Anchor Anchor;

        [XmlAttribute(AttributeName = "role")]
        public string Role { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
    }
}