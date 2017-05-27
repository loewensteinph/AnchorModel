using System.Text;

namespace Anchor.Model.Core.BusinessLogic
{
    public enum AttributeType
    {
        Static,
        Historized,
        StaticKnotted,
        HistorizedKnotted
    }

    public class Attribute : AnchorModel.Attribute
    {
        public Anchor Anchor;

        public AttributeType AttributeType;

        public Knot Knot;


        public string Identity => Anchor.Identity;


        public string AnchorIdentityColumnName => Anchor.IdentityColumnName;


        public string CreateTableStatement => GetCreateTableStatement();


        private string Name => $"{Mnemonic}_{Descriptor}";


        public string Capsule => Metadata.Capsule;


        public string IdentityColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{Anchor.Mnemonic}_ID";


        public string ChecksumColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_Checksum";


        public string KnotColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{Knot.Mnemonic}_ID";


        public string MetaDataColumnName => $"Metadata_{Anchor.Mnemonic}_{Mnemonic}";


        public string HistorizeColumn => $"{Anchor.Mnemonic}_{Mnemonic}_ChangedAt";


        public string AttributeColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{Anchor.Descriptor}_{Descriptor}";


        private string ValueColumnName => Name;

        public string TableName => $"{Anchor.Mnemonic}_{Mnemonic}_{Anchor.Descriptor}_{Descriptor}";


        private string IdentityGenerator => Metadata.Generator == "true" ? "IDENTITY(1,1)" : string.Empty;


        public bool HasCheckSum => Metadata.Checksum == "true";


        public bool IsKnotted => Knot != null;


        public bool IsHistorized => !string.IsNullOrEmpty(TimeRange);

        internal string GetCreateTableStatement()
        {
            var result = string.Empty;

            var sb = new StringBuilder();

            // ST_NAM_Stage_Name
            sb.AppendFormat(@"CREATE TABLE [{0}].[{1}] (", Capsule, TableName);
            sb.AppendFormat(@"{0} {1} {2} NOT NULL,", IdentityColumnName, Identity, IdentityGenerator);
            if (AttributeType.Equals(AttributeType.Historized) || AttributeType.Equals(AttributeType.Static))
                sb.AppendFormat(@"{0} {1} NOT NULL,", AttributeColumnName, DataRange);
            if (AttributeType.Equals(AttributeType.StaticKnotted) ||
                AttributeType.Equals(AttributeType.HistorizedKnotted))
                sb.AppendFormat(@"{0} {1} NOT NULL,", KnotColumnName, Knot.Identity);
            if (AttributeType.Equals(AttributeType.Historized) || AttributeType.Equals(AttributeType.HistorizedKnotted))
                sb.AppendFormat(@"{0} datetime NOT NULL,", HistorizeColumn);
            if (HasCheckSum)
                sb.AppendFormat(
                    "{0} AS cast(dbo.MD5(cast({1} as varbinary(max))) as varbinary(16)) PERSISTED,",
                    ChecksumColumnName, TableName);
            sb.AppendFormat(@"{0} int NOT NULL,", MetaDataColumnName);
            if (AttributeType.Equals(AttributeType.StaticKnotted) ||
                AttributeType.Equals(AttributeType.HistorizedKnotted))
                sb.AppendFormat(@"constraint fk_A_{0} foreign key (
        {1}
    ) references [{2}].[{3}]({4}),", AttributeColumnName, IdentityColumnName, Anchor.Metadata.Capsule, Anchor.Name,
                    Anchor.IdentityColumnName);
            if (AttributeType.Equals(AttributeType.Static) || AttributeType.Equals(AttributeType.Historized))
                sb.AppendFormat(@"constraint fk{0} foreign key (
        {1}
    ) references [{2}].[{3}]({4}),", AttributeColumnName, IdentityColumnName, Anchor.Metadata.Capsule, Anchor.Name,
                    Anchor.IdentityColumnName);
            if (AttributeType.Equals(AttributeType.StaticKnotted) ||
                AttributeType.Equals(AttributeType.HistorizedKnotted))
                sb.AppendFormat(@"constraint fk_K_{0} foreign key (
        {1}
    ) references [{2}].[{3}]({4}),", AttributeColumnName, KnotColumnName, Knot.Metadata.Capsule, Knot.Name,
                    Knot.IdentityColumnName);
            if (IsHistorized)
                sb.AppendFormat(@" constraint pk{0} primary key (
        {1} asc,
        {2} desc
    )", AttributeColumnName, IdentityColumnName, HistorizeColumn);
            if (!IsHistorized)
                sb.AppendFormat(@" constraint pk{0} primary key (
        {1} asc
    )", AttributeColumnName, IdentityColumnName);
            sb.Append(@");
GO");

            result = sb.ToString();
            return result;
        }
    }
}