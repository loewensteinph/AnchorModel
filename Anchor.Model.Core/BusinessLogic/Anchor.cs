using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anchor.Model.Core.BusinessLogic
{
    public class Anchor
    {
        public Anchor()
        {
            Attribute = new List<Attribute>();
        }

        public List<Attribute> Attribute { get; set; }
        private string TableName => $"{Mnemonic}_{Descriptor}";
        public string CreateTableStatement => GetCreateTableStatement();
        public string CreateLatestViewStatement => GetCreateLatestViewStatement();
        public string CreateInsertSpStatement => GetCreateInsertSpStatement();
        public string Name => $"{Mnemonic}_{Descriptor}";
        private string Capsule => Metadata.Capsule;
        public string IdentityColumnName => $"{Mnemonic}_ID";
        public string IdentityGenerator => Metadata.Generator == "true" ? "IDENTITY(1,1)" : string.Empty;
        public string Mnemonic { get; set; }
        public string Descriptor { get; set; }
        public string Identity { get; set; }
        public Metadata Metadata { get; set; }

        internal string GetCreateTableStatement()
        {
            var result = string.Empty;

            var sb = new StringBuilder();

            sb.AppendFormat(@"CREATE TABLE [{0}].[{1}] (
    {2} {3} {4} NOT NULL,", Capsule, Name, IdentityColumnName, Identity, IdentityGenerator);
            sb.AppendFormat(@"Metadata_{0} int not null,
    CONSTRAINT pk{0}_{1} primary key(
        {2} ASC
    ), ", Mnemonic, Descriptor, IdentityColumnName);
            sb.Append(@");
GO");
            result = sb.ToString();
            return result;
        }

        internal string GetCreateInsertSpStatement()
        {
            var result = string.Empty;

            var sb = new StringBuilder();

            sb.AppendFormat(@"CREATE PROCEDURE [{0}].[k{1}] (
        @requestedNumberOfIdentities BIGINT,
        @metadata INT
    ) AS
    BEGIN
        SET NOCOUNT ON;
        IF @requestedNumberOfIdentities > 0
        BEGIN
            WITH idGenerator (idNumber) AS (
                SELECT
                    1
                UNION ALL
                SELECT
                    idNumber + 1
                FROM
                    idGenerator
                WHERE
                    idNumber < @requestedNumberOfIdentities
            )", Capsule, Name);
            sb.AppendFormat(@"INSERT INTO [{0}].[{1}] (
                Metadata_{2}
            )
            OUTPUT
                inserted.{3}
            SELECT
                @metadata
            FROM
                idGenerator
            OPTION (MAXRECURSION 0);
        END
    END", Capsule, Name, Mnemonic, IdentityColumnName);
            result = sb.ToString();
            return result;
        }

        internal string GetCreateLatestViewStatement()
        {
            if (Attribute.Count == 0)
                return string.Empty;
            var result = string.Empty;

            var sb = new StringBuilder();
            sb.AppendFormat(@"CREATE VIEW [{0}].[l{1}] WITH SCHEMABINDING AS "
                , Capsule, Name);
            sb.AppendFormat(@"SELECT [{0}].{1},"
                , Mnemonic, IdentityColumnName);
            sb.AppendFormat(@"[{0}].Metadata_{1},"
                , Mnemonic, Mnemonic);
            foreach (var attribute in Attribute)
            {
                var last = attribute.Equals(Attribute.Last());
                sb.AppendFormat(@"[{0}].{1},"
                    , attribute.Mnemonic, attribute.IdentityColumnName);
                sb.AppendFormat(@"[{0}].{1},"
                    , attribute.Mnemonic, attribute.MetaDataColumnName);
                if (attribute.IsHistorized)
                    sb.AppendFormat(@"[{0}].{1},"
                        , attribute.Mnemonic, attribute.HistorizeColumn);
                if (attribute.HasCheckSum)
                    sb.AppendFormat(@"[{0}].{1},"
                        , attribute.Mnemonic, attribute.ChecksumColumnName);
                if (attribute.IsKnotted)
                {
                    sb.AppendFormat(@"[k{0}].{1} AS {2}_{3}_{4},"
                        , attribute.Mnemonic, attribute.Knot.Name, Mnemonic,
                        attribute.Mnemonic, attribute.Knot.Name);
                    sb.AppendFormat(@"[k{0}].Metadata_{1} AS {2}_{3}_Metadata_{4},"
                        , attribute.Mnemonic, attribute.Knot.Mnemonic, Mnemonic,
                        attribute.Mnemonic, attribute.Knot.Mnemonic);
                    sb.AppendFormat(@"[{0}].{1} {2}"
                        , attribute.Mnemonic, attribute.KnotColumnName, last ? string.Empty : ", "
                    );
                }
                else
                {
                    sb.AppendFormat(@"[{0}].{1}{2} "
                        , attribute.Mnemonic, attribute.AttributeColumnName, last ? string.Empty : ", ");
                }
            }
            sb.AppendFormat(@"FROM [{0}].[{1}] [{2}] "
                , Capsule, TableName, Mnemonic);
            foreach (var attribute in Attribute)
            {
                var last = attribute.Equals(Attribute.Last());
                sb.AppendFormat(@"LEFT JOIN [{0}].[{1}] [{2}] "
                    , attribute.Capsule, attribute.TableName, attribute.Mnemonic);
                sb.AppendFormat(@"ON [{0}].{1} = [{2}].{3} "
                    , attribute.Mnemonic, attribute.IdentityColumnName, Mnemonic, IdentityColumnName);
                if (attribute.IsHistorized)
                    sb.AppendFormat(@"AND
    [{0}].{1} = (
        SELECT
            max(sub.{1})
        FROM
            [{2}].[{3}] sub
        WHERE
            sub.{4} = [{5}].{6}
   ) "
                        , attribute.Mnemonic, attribute.HistorizeColumn, attribute.Metadata.Capsule,
                        attribute.TableName,
                        attribute.IdentityColumnName, Mnemonic, IdentityColumnName);
                /*
                 * LEFT JOIN
                    [dbo].[UTL_Utilization] [kAVG]
                ON
                    [kAVG].UTL_ID = [AVG].ST_AVG_UTL_ID
                 */
                if (attribute.IsKnotted)
                {
                    sb.AppendFormat(@"LEFT JOIN [{0}].[{1}] [k{2}] "
                        , attribute.Knot.Metadata.Capsule, attribute.Knot.Name, attribute.Mnemonic);
                    sb.AppendFormat(@"ON [k{0}].{1} = [{2}].{3} "
                        , attribute.Mnemonic, attribute.Knot.IdentityColumnName, attribute.Mnemonic,
                        attribute.KnotColumnName);
                }
            }
            result = sb.ToString();
            return result;
        }
    }
}