namespace Anchor.Model.Core.BusinessLogic
{
    public class Metadata
    {
        public bool improved => Naming == "improved";
        public string ChangingRange { get; set; }
        public string Encapsulation { get; set; }
        public string Identity { get; set; }
        public string MetadataPrefix { get; set; }
        public string MetadataType { get; set; }
        public string MetadataUsage { get; set; }
        public string ChangingSuffix { get; set; }
        public string IdentitySuffix { get; set; }
        public string PositIdentity { get; set; }
        public string PositGenerator { get; set; }
        public string PositingRange { get; set; }
        public string PositingSuffix { get; set; }
        public string PositorRange { get; set; }
        public string PositorSuffix { get; set; }
        public string ReliabilityRange { get; set; }
        public string ReliabilitySuffix { get; set; }
        public string DeleteReliability { get; set; }
        public string AssertionSuffix { get; set; }
        public string Partitioning { get; set; }
        public string EntityIntegrity { get; set; }
        public string Restatability { get; set; }
        public string Idempotency { get; set; }
        public string Assertiveness { get; set; }
        public string Naming { get; set; }
        public string PositSuffix { get; set; }
        public string AnnexSuffix { get; set; }
        public string Chronon { get; set; }
        public string Now { get; set; }
        public string DummySuffix { get; set; }
        public string VersionSuffix { get; set; }
        public string StatementTypeSuffix { get; set; }
        public string ChecksumSuffix { get; set; }
        public string BusinessViews { get; set; }
        public string Decisiveness { get; set; }
        public string Equivalence { get; set; }
        public string EquivalentSuffix { get; set; }
        public string EquivalentRange { get; set; }
        public string DatabaseTarget { get; set; }
        public string Temporalization { get; set; }
        public string Capsule { get; set; }
        public string Generator { get; set; }
        public string Checksum { get; set; }
        public string Restatable { get; set; }
        public string Idempotent { get; set; }
    }
}