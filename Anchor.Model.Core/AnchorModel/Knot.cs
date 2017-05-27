using System.Text;
using System.Xml.Serialization;

namespace Anchor.Model.Core.AnchorModel
{
    [XmlRoot(ElementName = "knot")]
    public class Knot
    {
        [XmlIgnore]
        public string CreateTableStatement => GetCreateTableStatement();

        [XmlIgnore]
        public string Name => $"{Mnemonic}_{Descriptor}";

        [XmlIgnore]
        private string Capsule => Metadata.Capsule;

        [XmlIgnore]
        public string IdentityColumnName => $"{Mnemonic}_ID";

        [XmlIgnore]
        private string ValueColumnName => Name;

        [XmlIgnore]
        private string IdentityGenerator => Metadata.Generator == "true" ? "IDENTITY(1,1)" : string.Empty;

        [XmlIgnore]
        private bool HasCheckSum => Metadata.Checksum == "true";

        [XmlElement(ElementName = "metadata")]
        public Metadata Metadata { get; set; }

        [XmlElement(ElementName = "layout")]
        public Layout Layout { get; set; }

        [XmlAttribute(AttributeName = "mnemonic")]
        public string Mnemonic { get; set; }

        [XmlAttribute(AttributeName = "descriptor")]
        public string Descriptor { get; set; }

        [XmlAttribute(AttributeName = "identity")]
        public string Identity { get; set; }

        [XmlAttribute(AttributeName = "dataRange")]
        public string DataRange { get; set; }

        internal string GetCreateTableStatement()
        {
            var result = string.Empty;

            var sb = new StringBuilder();

            sb.AppendFormat(@"CREATE TABLE [{0}].[{1}] (
    {2} {3} {4} NOT NULL,
    {5} {6} NOT NULL,", Capsule, Name, IdentityColumnName, Identity, IdentityGenerator, ValueColumnName, DataRange);
            if (HasCheckSum)
                sb.AppendFormat(
                    "{0}_Checksum AS cast(dbo.MD5(cast({1} as varbinary(max))) as varbinary(16)) PERSISTED,", Mnemonic,
                    ValueColumnName);
            sb.AppendFormat(@"Metadata_{0} int not null,
    CONSTRAINT pk{0}_{1} primary key(
        {2} ASC
    ), ", Mnemonic, Descriptor, IdentityColumnName);
            if (HasCheckSum)
                sb.AppendFormat(@"CONSTRAINT uq{0}_{1} UNIQUE (
        {0}_Checksum 
    )", Mnemonic, Descriptor);
            else
                sb.AppendFormat(@"CONSTRAINT uq{0}_{1} UNIQUE (
        {0}_{1} 
    )", Mnemonic, Descriptor);
            sb.Append(@");
GO");

            result = sb.ToString();
            return result;
        }
    }
}