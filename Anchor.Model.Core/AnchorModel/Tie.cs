using System.Collections.Generic;
using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    /*
    -- Ties are used to represent relationships between entities.
    -- They come in four flavors: static, historized, knotted static, and knotted historized.
    -- Ties have cardinality, constraining how members may participate in the relationship.
    -- Every entity that is a member in a tie has a specified role in the relationship.
    -- Ties must have at least two anchor roles and zero or more knot roles.
     */
    [XmlRoot(ElementName = "tie")]
    public class Tie
    {
        [XmlElement(ElementName = "anchorRole")]
        public List<AnchorRole> AnchorRole { get; set; }

        [XmlElement(ElementName = "knotRole")]
        public KnotRole KnotRole { get; set; }

        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }

        [XmlAttribute(AttributeName = "timeRange")]
        public string TimeRange { get; set; }
    }
}