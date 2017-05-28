using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anchor.Model.Core.BusinessLogic
{
    public enum TieType
    {
        Static,
        Historized,
        StaticKnotted,
        HistorizedKnotted
    }

    public class Tie
    {
        public TieType TieType;
        public List<AnchorRole> AnchorRole { get; set; }
        public KnotRole KnotRole { get; set; }
        public Metadata Metadata { get; set; }
        public Layout Layout { get; set; }
        public string TimeRange { get; set; }
        public string CreateTableStatement => GetCreateTableStatement();
        public string RestatementFinderFunctionStatement => GetRestatementFinderFunctionStatement();
        public List<ForeignKey> ForeignKeyList { get; set; }
        public string TableName { get; set; }
        public bool IsHistorized => !string.IsNullOrEmpty(TimeRange);
        public bool HasIdentifiers { get; set; }

        internal string GetCreateTableStatement()
        {
            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE TABLE [{0}].[{1}] (", Metadata.Capsule, TableName);
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"{0} {1} not null,", fk.ColumnName, fk.ColumnDataType);
            if (IsHistorized)
                sb.AppendFormat(@"{0}_ChangedAt datetime not null,", TableName);
            sb.AppendFormat(@"Metadata_{0} int not null,", TableName);
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"constraint {0}_fk{1} foreign key (
        {2}
    ) references {3}({4}), ", TableName, fk.ConstraintSuffix, fk.ColumnName, fk.ReferencedTableName,
                    fk.ReferencedColumnName);
            if (IsHistorized && !HasIdentifiers)
                foreach (var fk in ForeignKeyList.Where(fk => fk.ReferencedTableType.Equals("Anchor")))
                    sb.AppendFormat(@"constraint {0}_uq{1} unique (
        {2},
        {3}_ChangedAt
    ), ", TableName, fk.ConstraintSuffix, fk.ColumnName, TableName);
            if (!IsHistorized && !HasIdentifiers)
            {
                var anchorFK = ForeignKeyList.Where(fk => fk.ReferencedTableType.Equals("Anchor"));
                foreach (var fk in anchorFK)
                    sb.AppendFormat(@"constraint {0}_uq{1} unique (
        {2}
    ), ", TableName, fk.ConstraintSuffix, fk.ColumnName);
            }
            sb.AppendFormat(@"constraint pk{0} primary key(", TableName);
            if (!HasIdentifiers)
                foreach (var fk in ForeignKeyList)
                {
                    var islast = fk.Equals(ForeignKeyList.Last());
                    sb.AppendFormat(@"{0} asc{1}", fk.ColumnName, IsHistorized || !islast ? "," : string.Empty);
                }
            if (HasIdentifiers)
            {
                var identifierFK = ForeignKeyList.Where(fk => fk.IsIdentifier);
                foreach (var fk in identifierFK)
                {
                    var islast = fk.Equals(identifierFK.Last());
                    sb.AppendFormat(@"{0} asc{1}", fk.ColumnName, IsHistorized || !islast ? "," : string.Empty);
                }
            }
            if (IsHistorized)
                sb.AppendFormat(@"{0}_ChangedAt desc", TableName);

            sb.Append(@")");
            sb.Append(@");
GO");
            result = sb.ToString();
            return result;
        }

        internal string GetRestatementFinderFunctionStatement()
        {
            if (!IsHistorized || HasIdentifiers)
                return string.Empty;
            var result = string.Empty;
            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE FUNCTION [{0}].[rf{1}] (", Metadata.Capsule, TableName);
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"@{0} {1},", fk.ColumnName, fk.ColumnDataType);
            sb.Append(@"@changed DATETIME
            )");
            sb.Append(@" RETURNS TINYINT AS
    BEGIN RETURN (
        SELECT
            COUNT(*)
        FROM (
            SELECT TOP 1");
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"pre.{0}{1}", fk.ColumnName, fk.Equals(ForeignKeyList.Last()) ? string.Empty : ",");
            sb.AppendFormat(@" FROM
                [{0}].[{1}] pre
            WHERE
            (", Metadata.Capsule, TableName);

            var anchorFK = ForeignKeyList.Where(fk => fk.ReferencedTableType.Equals("Anchor"));

            foreach (var fk in anchorFK)
            {
                sb.AppendFormat(@"pre.{0} = @{0}
{1}", fk.ColumnName, fk.Equals(anchorFK.Last()) ? string.Empty : "OR ");
            }
            sb.AppendFormat(@")
            AND
                pre.{0}_ChangedAt < @changed
            ORDER BY
                pre.{0}_ChangedAt DESC
            UNION", TableName);
            sb.Append(@"
            SELECT TOP 1");
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"fol.{0}{1}", fk.ColumnName, fk.Equals(ForeignKeyList.Last()) ? string.Empty : ",");
            sb.AppendFormat(@" FROM
                [{0}].[{1}] fol
            WHERE
            (", Metadata.Capsule, TableName);
            foreach (var fk in anchorFK)
            {
                sb.AppendFormat(@"fol.{0} = @{0}
{1}", fk.ColumnName, fk.Equals(anchorFK.Last()) ? string.Empty : "OR ");
            }
            sb.AppendFormat(@")
            AND
                fol.{0}_ChangedAt > @changed
            ORDER BY
                fol.{0}_ChangedAt ASC
            ) s
            WHERE ", TableName);
            foreach (var fk in ForeignKeyList)
                sb.AppendFormat(@"s.{0} = @{0} {1}", fk.ColumnName,
                    fk.Equals(ForeignKeyList.Last()) ? string.Empty : "AND ");
            sb.Append(@");
    END
GO");
            result = sb.ToString();
            return result;
        }
    }
}