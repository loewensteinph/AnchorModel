using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [Serializable]
    [XmlRoot(ElementName = "schema")]
    public class Schema
    {
        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "knot")]
        public List<Knot> Knot { get; set; }

        [XmlElement(ElementName = "anchor")]
        public List<Anchor> Anchor { get; set; }

        [XmlElement(ElementName = "tie")]
        public List<Tie> Tie { get; set; }

        [XmlAttribute(AttributeName = "format")]
        public string Format { get; set; }

        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }

        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
    }
}