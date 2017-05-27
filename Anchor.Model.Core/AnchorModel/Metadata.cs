using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "metadata")]
    public class Metadata
    {
        [XmlAttribute(AttributeName = "changingRange")]
        public string ChangingRange { get; set; }

        [XmlAttribute(AttributeName = "encapsulation")]
        public string Encapsulation { get; set; }

        [XmlAttribute(AttributeName = "identity")]
        public string Identity { get; set; }

        [XmlAttribute(AttributeName = "metadataPrefix")]
        public string MetadataPrefix { get; set; }

        [XmlAttribute(AttributeName = "metadataType")]
        public string MetadataType { get; set; }

        [XmlAttribute(AttributeName = "metadataUsage")]
        public string MetadataUsage { get; set; }

        [XmlAttribute(AttributeName = "changingSuffix")]
        public string ChangingSuffix { get; set; }

        [XmlAttribute(AttributeName = "identitySuffix")]
        public string IdentitySuffix { get; set; }

        [XmlAttribute(AttributeName = "positIdentity")]
        public string PositIdentity { get; set; }

        [XmlAttribute(AttributeName = "positGenerator")]
        public string PositGenerator { get; set; }

        [XmlAttribute(AttributeName = "positingRange")]
        public string PositingRange { get; set; }

        [XmlAttribute(AttributeName = "positingSuffix")]
        public string PositingSuffix { get; set; }

        [XmlAttribute(AttributeName = "positorRange")]
        public string PositorRange { get; set; }

        [XmlAttribute(AttributeName = "positorSuffix")]
        public string PositorSuffix { get; set; }

        [XmlAttribute(AttributeName = "reliabilityRange")]
        public string ReliabilityRange { get; set; }

        [XmlAttribute(AttributeName = "reliabilitySuffix")]
        public string ReliabilitySuffix { get; set; }

        [XmlAttribute(AttributeName = "deleteReliability")]
        public string DeleteReliability { get; set; }

        [XmlAttribute(AttributeName = "assertionSuffix")]
        public string AssertionSuffix { get; set; }

        [XmlAttribute(AttributeName = "partitioning")]
        public string Partitioning { get; set; }

        [XmlAttribute(AttributeName = "entityIntegrity")]
        public string EntityIntegrity { get; set; }

        [XmlAttribute(AttributeName = "restatability")]
        public string Restatability { get; set; }

        [XmlAttribute(AttributeName = "idempotency")]
        public string Idempotency { get; set; }

        [XmlAttribute(AttributeName = "assertiveness")]
        public string Assertiveness { get; set; }

        [XmlAttribute(AttributeName = "naming")]
        public string Naming { get; set; }

        [XmlAttribute(AttributeName = "positSuffix")]
        public string PositSuffix { get; set; }

        [XmlAttribute(AttributeName = "annexSuffix")]
        public string AnnexSuffix { get; set; }

        [XmlAttribute(AttributeName = "chronon")]
        public string Chronon { get; set; }

        [XmlAttribute(AttributeName = "now")]
        public string Now { get; set; }

        [XmlAttribute(AttributeName = "dummySuffix")]
        public string DummySuffix { get; set; }

        [XmlAttribute(AttributeName = "versionSuffix")]
        public string VersionSuffix { get; set; }

        [XmlAttribute(AttributeName = "statementTypeSuffix")]
        public string StatementTypeSuffix { get; set; }

        [XmlAttribute(AttributeName = "checksumSuffix")]
        public string ChecksumSuffix { get; set; }

        [XmlAttribute(AttributeName = "businessViews")]
        public string BusinessViews { get; set; }

        [XmlAttribute(AttributeName = "decisiveness")]
        public string Decisiveness { get; set; }

        [XmlAttribute(AttributeName = "equivalence")]
        public string Equivalence { get; set; }

        [XmlAttribute(AttributeName = "equivalentSuffix")]
        public string EquivalentSuffix { get; set; }

        [XmlAttribute(AttributeName = "equivalentRange")]
        public string EquivalentRange { get; set; }

        [XmlAttribute(AttributeName = "databaseTarget")]
        public string DatabaseTarget { get; set; }

        [XmlAttribute(AttributeName = "temporalization")]
        public string Temporalization { get; set; }

        [XmlAttribute(AttributeName = "capsule")]
        public string Capsule { get; set; }

        [XmlAttribute(AttributeName = "generator")]
        public string Generator { get; set; }

        [XmlAttribute(AttributeName = "checksum")]
        public string Checksum { get; set; }

        [XmlAttribute(AttributeName = "restatable")]
        public string Restatable { get; set; }

        [XmlAttribute(AttributeName = "idempotent")]
        public string Idempotent { get; set; }
    }
}