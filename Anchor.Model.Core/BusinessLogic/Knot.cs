using System.Text;

namespace Anchor.Model.Core.BusinessLogic
{
    public class Knot
    {
        public Metadata Metadata { get; set; }

        public Layout Layout { get; set; }

        public string Mnemonic { get; set; }
        public string Descriptor { get; set; }

        public string Identity { get; set; }

        public string DataRange { get; set; }

        public string CreateTableStatement => GetCreateTableStatement();
        public string Name => $"{Mnemonic}_{Descriptor}";

        private string Capsule => Metadata.Capsule;

        public string IdentityColumnName => $"{Mnemonic}_ID";

        private string ValueColumnName => Name;

        private string IdentityGenerator => Metadata.Generator == "true" ? "IDENTITY(1,1)" : string.Empty;

        private bool HasCheckSum => Metadata.Checksum == "true";

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