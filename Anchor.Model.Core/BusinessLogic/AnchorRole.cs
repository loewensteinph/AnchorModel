using System.Xml.Serialization;

namespace Anchor.Model.Core.BusinessLogic
{
    [XmlRoot(ElementName = "anchorRole")]
    public class AnchorRole
    {
        public Anchor Anchor;
        public string Role { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
    }
}