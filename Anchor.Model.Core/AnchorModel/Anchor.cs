using System.Collections.Generic;
using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "anchor")]
    /*
        -- Anchors are used to store the identities of entities.
        -- Anchors are immutable.
        -- Anchors may have zero or more adjoined attributes.
     */
    public class Anchor
    {
        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "attribute")]
        public List<Attribute> Attribute { get; set; }

        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }

        [XmlAttribute(AttributeName = "mnemonic")]
        public string Mnemonic { get; set; }

        [XmlAttribute(AttributeName = "descriptor")]
        public string Descriptor { get; set; }

        [XmlAttribute(AttributeName = "identity")]
        public string Identity { get; set; }
    }
}