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
        public Metadata GlobalMetadata;
        public Knot Knot;
        private string statementTypes => IsHistorized && !IsIdempotent ? "'N','R'" : "'N'";
        private bool IsIdempotent => Metadata.Idempotent == "true" ? true : false;
        private string identityGenerator => Metadata.Generator == "true" ? "IDENTITY(1,1)" : string.Empty;
        private string uniqueMnemonic => $"{Anchor.Mnemonic}_{Mnemonic}";
        private string name => $"{Anchor.Mnemonic}_{Anchor.Descriptor}_{Descriptor}";
        private string businessName => Descriptor;
        private string positName => $"{Anchor.Mnemonic}_{GlobalMetadata.PositSuffix}";
        private string annexName => $"{name}_{GlobalMetadata.AnnexSuffix}";
        public string checksumColumnName => $"{uniqueMnemonic}_{GlobalMetadata.ChecksumSuffix}";
        public string identityColumnName => $"{uniqueMnemonic}_{GlobalMetadata.IdentitySuffix}";
        public string metadataColumnName => $"{GlobalMetadata.MetadataPrefix}_{uniqueMnemonic}";
        private string equivalentColumnName => $"{uniqueMnemonic}_{GlobalMetadata.EquivalentSuffix}";
        private string versionColumnName => $"{uniqueMnemonic}_{GlobalMetadata.VersionSuffix}";
        private string positingColumnName => $"{uniqueMnemonic}_{GlobalMetadata.PositingSuffix}";
        private string positorColumnName => $"{uniqueMnemonic}_{GlobalMetadata.PositorSuffix}";
        private string reliabilityColumnName => $"{uniqueMnemonic}_{GlobalMetadata.ReliabilitySuffix}";

        private string reliableColumnName =>
            $"{uniqueMnemonic}_{GlobalMetadata.ReliabilitySuffix}"; // TODO:Check Matadata!

        private string statementTypeColumnName => $"{uniqueMnemonic}_{GlobalMetadata.StatementTypeSuffix}";

        public string anchorReferenceName => GlobalMetadata.improved
            ? $"{uniqueMnemonic}_{Anchor.Mnemonic}_{GlobalMetadata.IdentitySuffix}"
            : Anchor.identityColumnName;

        private string knotReferenceName => GlobalMetadata.improved
            ? $"{uniqueMnemonic}_{KnotRange}_{GlobalMetadata.IdentitySuffix}"
            : $"{KnotRange}_{GlobalMetadata.IdentitySuffix}";

        public string knotValueColumnName => GlobalMetadata.improved ? $"{uniqueMnemonic}_{Knot.Name}" : Knot.Name;
        private string knotChecksumColumnName => $"{uniqueMnemonic}_{Knot.checksumColumnName}";

        private string knotMetadataColumnName => GlobalMetadata.improved
            ? $"{uniqueMnemonic}_{Knot.metadataColumnName}"
            : Knot.metadataColumnName;

        private string knotBusinessName => $"{businessName}_{Knot.businessName}";
        private string valueColumnName => knotReferenceName == string.Empty ? name : knotReferenceName;
        public string changingColumnName => $"{uniqueMnemonic}_{GlobalMetadata.ChangingSuffix}";
        public string Identity => Anchor.Identity;
        public string CreateTableStatement => GetCreateTableStatement();
        public string InsertTriggerStatement => GetInsertTriggerStatement();
        public string RestatementFinderFunctionStatement => GetRestatementFinderFunctionStatement();
        public string RewinderFunctionStatement => GetRewinderFunctionStatement();
        public string Capsule => Metadata.Capsule;
        public string ChecksumColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{GlobalMetadata.ChecksumSuffix}";
        public string KnotColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{Knot.Mnemonic}_ID";
        public string MetaDataColumnName => $"Metadata_{Anchor.Mnemonic}_{Mnemonic}";
        public string HistorizeColumn => $"{Anchor.Mnemonic}_{Mnemonic}_ChangedAt";
        public string AttributeColumnName => $"{Anchor.Mnemonic}_{Mnemonic}_{Anchor.Descriptor}_{Descriptor}";
        public string TableName => $"{Anchor.Mnemonic}_{Mnemonic}_{Anchor.Descriptor}_{Descriptor}";
        public bool HasCheckSum => Metadata.Checksum == "true";
        public bool IsKnotted => Knot != null;
        public bool IsHistorized => !string.IsNullOrEmpty(TimeRange);
        public bool IsEquivalent => Metadata.Equivalence == "true";
        private string valueType => IsKnotted ? Knot.Identity : HasCheckSum ? "varbinary(16)" : DataRange;

        internal string GetCreateTableStatement()
        {
            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE TABLE [{0}].[{1}] (", Capsule, TableName);
            sb.AppendFormat(@"{0} {1} {2} NOT NULL,", anchorReferenceName, Identity, identityGenerator);
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
    ) references [{2}].[{3}]({4}),", AttributeColumnName, anchorReferenceName, Anchor.capsule, Anchor.Name,
                    Anchor.IdentityColumnName);
            if (AttributeType.Equals(AttributeType.Static) || AttributeType.Equals(AttributeType.Historized))
                sb.AppendFormat(@"constraint fk{0} foreign key (
        {1}
    ) references [{2}].[{3}]({4}),", AttributeColumnName, anchorReferenceName, Anchor.capsule, Anchor.Name,
                    Anchor.identityColumnName);
            if (AttributeType.Equals(AttributeType.StaticKnotted) ||
                AttributeType.Equals(AttributeType.HistorizedKnotted))
                sb.AppendFormat(@"constraint fk_K_{0} foreign key (
        {1}
    ) references [{2}].[{3}]({4}),", AttributeColumnName, knotReferenceName, Knot.capsule, Knot.Name,
                    Knot.identityColumnName);
            if (IsHistorized)
                sb.AppendFormat(@" constraint pk{0} primary key (
        {1} asc,
        {2} desc
    )", AttributeColumnName, anchorReferenceName, HistorizeColumn);
            if (!IsHistorized)
                sb.AppendFormat(@" constraint pk{0} primary key (
        {1} asc
    )", AttributeColumnName, anchorReferenceName);
            sb.Append(@");
GO");
            result = sb.ToString();
            return result;
        }

        internal string GetInsertTriggerStatement()
        {
            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE TRIGGER [{0}].[it_{1}] ON [{0}].[{1}]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxVersion int;
    DECLARE @currentVersion int;", Capsule, TableName);
            sb.AppendFormat(@"DECLARE @{0} TABLE (
        {1} {2} not null,
        ", TableName, anchorReferenceName, Identity);
            if (IsEquivalent)
                sb.AppendFormat(@"{0} {1} not null,", equivalentColumnName, GlobalMetadata.EquivalentRange);
            sb.AppendFormat(@"{0} int not null,
        ", MetaDataColumnName);
            if (IsHistorized)
                sb.AppendFormat(@"{0} datetime not null,
       ", changingColumnName);
            if (IsKnotted)
                sb.AppendFormat(@"{0} {1} not null,", knotReferenceName, Knot.Identity);
            else
                sb.AppendFormat(@"{0} {1} not null,", TableName, DataRange);
            if (HasCheckSum)
                sb.AppendFormat(@"{0} varbinary(16) not null,", checksumColumnName);
            sb.AppendFormat(@"{0} bigint not null,", versionColumnName);
            sb.AppendFormat(@"{0} char(1) not null,", statementTypeColumnName);
            sb.AppendFormat(@"primary key(
            {0},
            {1})
);", versionColumnName, anchorReferenceName);
            sb.AppendFormat(@"INSERT INTO @{0}
    SELECT
        i.{1},", TableName, anchorReferenceName);
            if (IsEquivalent)
                sb.AppendFormat(@"i.{0},", equivalentColumnName);
            sb.AppendFormat(@"i.{0},", metadataColumnName);
            if (IsHistorized)
                sb.AppendFormat(@"i.{0},", changingColumnName);
            sb.AppendFormat(@"i.{0},", IsKnotted ? knotReferenceName : TableName);
            if (HasCheckSum)
                sb.AppendFormat(@"{0}.MD5(cast(i.{1} as varbinary(max))),", Capsule, TableName);
            if (IsHistorized)
                sb.AppendFormat(@"DENSE_RANK() OVER (
            PARTITION BY
                {0}
                i.{1}
            ORDER BY
                i.{2} ASC
        ),", IsEquivalent ? $"i.{equivalentColumnName}," : string.Empty, anchorReferenceName, changingColumnName);
            else
                sb.AppendFormat(@"ROW_NUMBER() OVER (
            PARTITION BY
                {0}
                i.{1}
            ORDER BY
                (SELECT 1) ASC -- some undefined order
        ),", IsEquivalent ? $"i.{equivalentColumnName}," : string.Empty, anchorReferenceName);
            sb.AppendFormat(@"'X'
    FROM
        inserted i;

    SELECT
        @maxVersion = {0},
        @currentVersion = 0
    FROM
        @{1};
    WHILE (@currentVersion < @maxVersion)
    BEGIN
        SET @currentVersion = @currentVersion + 1;
        UPDATE v
        SET
            v.{2} =
                CASE
                    WHEN [{3}].{4} is not null
                    THEN 'D' -- duplicate", IsHistorized ? $"max({versionColumnName})" : "1", TableName,
                statementTypeColumnName, Mnemonic, anchorReferenceName);
            if (IsHistorized)
                sb.AppendFormat(@"
                    WHEN [{0}].[rf{1}](
                        v.{2},
                        {3}
                        {4}
                        v.{5}
                    ) = 1
                    THEN 'R' -- restatement", Capsule, TableName, anchorReferenceName,
                    IsEquivalent ? $"v.{equivalentColumnName}," : string.Empty
                    , HasCheckSum ? $"v.{checksumColumnName}," : IsKnotted ? $"v.{knotReferenceName}," : $"v.{TableName},",
                    changingColumnName);
            sb.AppendFormat(@"
                    ELSE 'N' -- new statement
                END
        FROM
            @{0} v
        LEFT JOIN
            [{1}].[{2}] [{3}]
        ON
            [{3}].{4} = v.{4}", TableName, Capsule, TableName, Mnemonic, anchorReferenceName);
            if (IsHistorized)
                sb.AppendFormat(@"
        AND
            [{0}].{1} = v.{1}
", Mnemonic, changingColumnName);
            if (IsEquivalent)
                sb.AppendFormat(@"
        AND
            [{0}].{1} = v.{1}
", Mnemonic, equivalentColumnName);
            if (HasCheckSum)
                sb.AppendFormat(@"
        AND
            [{0}].{1} = v.{1}
", Mnemonic, checksumColumnName);
            else
                sb.AppendFormat(@"
        AND
            [{0}].{1} = v.{1}
", Mnemonic, IsKnotted ? knotReferenceName : TableName);
            sb.AppendFormat(@"
        WHERE
            v.{0} = @currentVersion;
        INSERT INTO [{1}].[{2}] (
            {3},", versionColumnName, Capsule, TableName, anchorReferenceName);
            if (IsEquivalent)
                sb.AppendFormat(@"{0},", equivalentColumnName);
            sb.AppendFormat(@"{0},", metadataColumnName);
            if (IsHistorized)
                sb.AppendFormat(@"{0},", changingColumnName);
            sb.AppendFormat(@"{0}", IsKnotted ? knotReferenceName : TableName);
            sb.AppendFormat(@")
        SELECT
            {0},", anchorReferenceName);
            if (IsEquivalent)
                sb.AppendFormat(@"{0},", equivalentColumnName);
            sb.AppendFormat(@"{0},", metadataColumnName);
            if (IsHistorized)
                sb.AppendFormat(@"{0},", changingColumnName);
            sb.AppendFormat(@"{0} 
        FROM
            @{1} 
        WHERE
            {2}  = @currentVersion
        AND
            {3} in ({4});
    END
END
GO", IsKnotted ? knotReferenceName : TableName, TableName, versionColumnName, statementTypeColumnName, statementTypes);
            result = sb.ToString();
            return result;
        }

        internal string GetRestatementFinderFunctionStatement()
        {
            if (!IsHistorized)
                return string.Empty;

            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE FUNCTION [{0}].[rf{1}] (
        @id {2},", Capsule, TableName, Anchor.Identity);
            if (IsEquivalent)
                sb.AppendFormat(@"@eq {0} ,", GlobalMetadata.EquivalentRange);
            sb.AppendFormat(@"@value {0} ,
            @changed {1}"
                , valueType, TimeRange);
            sb.AppendFormat(@")
    RETURNS tinyint AS
    BEGIN RETURN (
        CASE WHEN EXISTS (
            SELECT
                @value 
            WHERE
                @value = (
                    SELECT TOP 1
                        pre.{0}
                    FROM
                        {1} pre
                    WHERE
                        pre.{2} = @id
                    AND
                        pre.{3} < @changed
                    ORDER BY
                        pre.{3} DESC
                )
        ) OR EXISTS (
            SELECT
                @value 
            WHERE
                @value = (
                    SELECT TOP 1
                        fol.{0}
                    FROM
                        {1} fol
                    WHERE
                        fol.{2} = @id
                    AND
                        fol.{3} > @changed
                    ORDER BY
                        fol.{3} ASC
                )
        )
        THEN 1
        ELSE 0
        END
    );
    END", IsKnotted ? knotReferenceName : TableName, IsEquivalent
                    ? $"[{Capsule}].[e{TableName}](@eq)"
                    : $"[{Capsule}].[{TableName}]"
                , anchorReferenceName, changingColumnName);

            result = sb.ToString();
            return result;
        }

        internal string GetRewinderFunctionStatement()
        {
            if (!IsHistorized)
                return string.Empty;

            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE FUNCTION [{0}].[r{3}] (
            {1}
        @changingTimepoint {2}
    )
    RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT
", Capsule, IsEquivalent ? $"@equivalent {GlobalMetadata.EquivalentRange}," : string.Empty, TimeRange,TableName);
            sb.AppendFormat(@"{0},
        {1},
        {2}
        {3}
        {4},
        {5}
    FROM
        {6}
    WHERE
        {5} <= @changingTimepoint;
GO", metadataColumnName, anchorReferenceName, IsEquivalent ? $"{equivalentColumnName}," : string.Empty,
                HasCheckSum ? $"{checksumColumnName}," : string.Empty,
                IsKnotted && !HasCheckSum ? knotReferenceName : TableName
                , changingColumnName,
                IsEquivalent ? $"[{Capsule}].[e{TableName}](@equivalent)," : $"[{Capsule}].[{TableName}]");
            result = sb.ToString();
            return result;
        }
    }
}