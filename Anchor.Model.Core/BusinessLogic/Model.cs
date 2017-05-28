﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Anchor.Model.Core.Helper;
using AutoMapper;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Schema = Anchor.Model.Core.AnchorModel.Schema;

namespace Anchor.Model.Core.BusinessLogic
{
    public class Model
    {
        public TSqlModel SqlModel;

        public Model(string path)
        {
            Anchor = new List<Anchor>();
            Knot = new List<Knot>();
            Tie = new List<Tie>();
            Schema = new Schema();
            Metadata = new Metadata();

            var serializer = new XmlSerializer(typeof(Schema));
            var reader = new StreamReader(path);
            Schema = (Schema) serializer.Deserialize(reader);
            reader.Close();

            Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<AnchorModel.Metadata, Metadata>();
                    cfg.CreateMap<AnchorModel.Anchor, Anchor>();
                    cfg.CreateMap<AnchorModel.Attribute, Attribute>();
                    cfg.CreateMap<AnchorModel.Knot, Knot>();
                    cfg.CreateMap<AnchorModel.Tie, Tie>();
                    cfg.CreateMissingTypeMaps = true;
                }
            );

            Metadata = Mapper.Map<Metadata>(Schema.Metadata);

            foreach (var anch in Schema.Anchor)
            {
                var anchor = Mapper.Map<Anchor>(anch);

                Anchor.Add(anchor);
            }
            foreach (var kn in Schema.Knot)
            {
                var knot = Mapper.Map<Knot>(kn);
                Knot.Add(knot);
            }
            foreach (var t in Schema.Tie)
            {
                var tie = Mapper.Map<Tie>(t);
                Tie.Add(tie);
            }
            InitializeSchema();
        }

        public Metadata Metadata { get; set; }
        public Schema Schema { get; set; }
        public List<Anchor> Anchor { get; set; }
        public List<Tie> Tie { get; set; }
        public List<Knot> Knot { get; set; }
        public List<Attribute> Attribute { get; set; }

        public void GenerateDacPac()
        {
            DacPackageExtensions.BuildPackage(
                "AnchorDacPac.dacpac",
                SqlModel,
                new PackageMetadata
                {
                    Name = "MyPackageName",
                    Description = "This is usually ignored",
                    Version = "1.0"
                },
                new PackageOptions());
        }

        private void SetAnchorReferences()
        {
            foreach (var anchor in Anchor)
            {
                anchor.GlobalMetadata = Metadata;
                foreach (var attribute in anchor.Attribute)
                {
                    attribute.GlobalMetadata = Metadata;
                    attribute.Anchor = anchor;
                    if (!string.IsNullOrEmpty(attribute.KnotRange))
                        attribute.Knot = Knot.FirstOrDefault(kn => kn.Mnemonic == attribute.KnotRange);
                    if (!attribute.IsHistorized && !attribute.IsKnotted)
                        attribute.AttributeType = AttributeType.Static;
                    if (attribute.IsHistorized && !attribute.IsKnotted)
                        attribute.AttributeType = AttributeType.Historized;
                    if (!attribute.IsHistorized && attribute.IsKnotted)
                        attribute.AttributeType = AttributeType.StaticKnotted;
                    if (attribute.IsHistorized && attribute.IsKnotted)
                        attribute.AttributeType = AttributeType.HistorizedKnotted;
                }
            }
        }

        private void SetTieReferences()
        {
            foreach (var tie in Tie)
            {
                if (tie.AnchorRole.Any())
                    tie.ForeignKeyList = new List<ForeignKey>();
                var sb = new StringBuilder();
                foreach (var anchorRole in tie.AnchorRole)
                {
                    if (tie.AnchorRole.Any(ar => ar.Identifier.Equals("true")))
                        tie.HasIdentifiers = true;

                    var anchor = Anchor.FirstOrDefault(an => an.Mnemonic == anchorRole.Type);
                    anchorRole.Anchor = anchor;

                    tie.ForeignKeyList.Add(new ForeignKey
                    {
                        ColumnName = $"{anchor.IdentityColumnName}_{anchorRole.Role}",
                        ColumnDataType = anchor.Identity,
                        ConstraintSuffix = $"{anchor.Mnemonic}_{anchorRole.Role}",
                        ReferencedColumnName = anchor.IdentityColumnName,
                        ReferencedTableName = $"[{anchor.Metadata.Capsule}].[{anchor.Name}]",
                        ReferencedTableType = typeof(Anchor).Name,
                        IsIdentifier = anchorRole.Identifier.Equals("true")
                    });

                    if (tie.AnchorRole.First().Equals(anchorRole) && sb.Length == 0)
                        sb.AppendFormat("{0}_{1}", anchor.Mnemonic, anchorRole.Role);
                    if (!tie.AnchorRole.First().Equals(anchorRole) && sb.Length > 0)
                        sb.AppendFormat("_{0}_{1}", anchor.Mnemonic, anchorRole.Role);
                }
                if (tie.KnotRole != null)
                {
                    var knot = Knot.FirstOrDefault(kn => kn.Mnemonic == tie.KnotRole.Type);
                    tie.KnotRole.Knot = knot;

                    if (tie.KnotRole.Equals("true"))
                        tie.HasIdentifiers = true;

                    tie.ForeignKeyList.Add(new ForeignKey
                    {
                        ColumnName = $"{knot.IdentityColumnName}_{tie.KnotRole.Role}",
                        ColumnDataType = knot.Identity,
                        ConstraintSuffix = $"{knot.Mnemonic}_{tie.KnotRole.Role}",
                        ReferencedColumnName = knot.IdentityColumnName,
                        ReferencedTableName = $"[{knot.Metadata.Capsule}].[{knot.Name}]",
                        ReferencedTableType = typeof(Knot).Name,
                        IsIdentifier = tie.KnotRole.Identifier.Equals("true")
                    });

                    if (sb.Length == 0)
                        sb.AppendFormat("{0}_{1}", knot.Mnemonic, tie.KnotRole.Role);
                    sb.AppendFormat("_{0}_{1}", knot.Mnemonic, tie.KnotRole.Role);
                }
                tie.TableName = sb.ToString();
            }
        }

        private void SetKnotReferences()
        {
            foreach (var knot in Knot)
                knot.GlobalMetadata = Metadata;
        }

        public void InitializeSchema()
        {
            SetAnchorReferences();

            SetTieReferences();

            SetKnotReferences();

            var options = new TSqlModelOptions();
            SqlModel = new TSqlModel(SqlServerVersion.Sql130, options);

            SqlModel.AddObjects(@"BEGIN
    CREATE ASSEMBLY Anchor
    AUTHORIZATION dbo
    -- you can use the DLL instead if you substitute for your path:
    -- FROM 'path_to_Anchor.dll'
    -- or you can use the binary representation of the compiled code:
    FROM 0x4D5A90000300000004000000FFFF0000B800000000000000400000000000000000000000000000000000000000000000000000000000000000000000800000000E1FBA0E00B409CD21B8014CCD21546869732070726F6772616D2063616E6E6F742062652072756E20696E20444F53206D6F64652E0D0D0A2400000000000000504500004C010300E7B633540000000000000000E00002210B010800000600000006000000000000CE2500000020000000400000000040000020000000020000040000000000000004000000000000000080000000020000000000000300408500001000001000000000100000100000000000001000000000000000000000007C2500004F00000000400000A002000000000000000000000000000000000000006000000C00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200000080000000000000000000000082000004800000000000000000000002E74657874000000D4050000002000000006000000020000000000000000000000000000200000602E72737263000000A0020000004000000004000000080000000000000000000000000000400000402E72656C6F6300000C0000000060000000020000000C00000000000000000000000000004000004200000000000000000000000000000000B025000000000000480000000200050080200000FC040000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000096026F0400000A2D16280500000A026F0600000A6F0700000A730800000A2A14280900000A2A1E02280A00000A2A000042534A4201000100000000000C00000076322E302E35303732370000000005006C00000048010000237E0000B40100009001000023537472696E6773000000004403000008000000235553004C0300001000000023475549440000005C030000A001000023426C6F620000000000000002000001471500000900000000FA01330016000001000000090000000200000002000000010000000A00000003000000010000000200000000000A0001000000000006002F0028000A00570042000A00610042000600A30083000600C30083000A000301E800060040012301060055014B010600670123010000000001000000000001000100010010001500000005000100010050200000000096006A000A000100762000000000861872001100020000000100780021007200150029007200110031007200110019001801550139004401590119005C015E01490075016301110072006A0111008101700109007200110020001B001A002E000B0077012E0013008001048000000000000000000000000000000000E100000002000000000000000000000001001F000000000002000000000000000000000001003600000000000000003C4D6F64756C653E00416E63686F722E646C6C005574696C6974696573006D73636F726C69620053797374656D004F626A6563740053797374656D2E446174610053797374656D2E446174612E53716C54797065730053716C42696E6172790053716C427974657300486173684D4435002E63746F720062696E617279446174610053797374656D2E52756E74696D652E436F6D70696C6572536572766963657300436F6D70696C6174696F6E52656C61786174696F6E734174747269627574650052756E74696D65436F6D7061746962696C69747941747472696275746500416E63686F72004D6963726F736F66742E53716C5365727665722E5365727665720053716C46756E6374696F6E417474726962757465006765745F49734E756C6C0053797374656D2E53656375726974792E43727970746F677261706879004D4435004372656174650053797374656D2E494F0053747265616D006765745F53747265616D0048617368416C676F726974686D00436F6D7075746548617368006F705F496D706C69636974000000000003200000000000C7641B1E7755B04A8FCCCA2F22950BF30008B77A5C561934E0890600011109120D0320000104200101088139010003005455794D6963726F736F66742E53716C5365727665722E5365727665722E446174614163636573734B696E642C2053797374656D2E446174612C2056657273696F6E3D322E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038390A446174614163636573730000000054020F497344657465726D696E69737469630154557F4D6963726F736F66742E53716C5365727665722E5365727665722E53797374656D446174614163636573734B696E642C2053797374656D2E446174612C2056657273696F6E3D322E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038391053797374656D446174614163636573730000000003200002040000121D04200012210620011D051221052001011D0506000111091D050801000800000000001E01000100540216577261704E6F6E457863657074696F6E5468726F77730100A42500000000000000000000BE250000002000000000000000000000000000000000000000000000B0250000000000000000000000005F436F72446C6C4D61696E006D73636F7265652E646C6C0000000000FF2500204000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100100000001800008000000000000000000000000000000100010000003000008000000000000000000000000000000100000000004800000058400000440200000000000000000000440234000000560053005F00560045005200530049004F004E005F0049004E0046004F0000000000BD04EFFE00000100000000000000000000000000000000003F000000000000000400000002000000000000000000000000000000440000000100560061007200460069006C00650049006E0066006F00000000002400040000005400720061006E0073006C006100740069006F006E00000000000000B004A4010000010053007400720069006E006700460069006C00650049006E0066006F0000008001000001003000300030003000300034006200300000002C0002000100460069006C0065004400650073006300720069007000740069006F006E000000000020000000300008000100460069006C006500560065007200730069006F006E000000000030002E0030002E0030002E003000000038000B00010049006E007400650072006E0061006C004E0061006D006500000041006E00630068006F0072002E0064006C006C00000000002800020001004C006500670061006C0043006F00700079007200690067006800740000002000000040000B0001004F0072006900670069006E0061006C00460069006C0065006E0061006D006500000041006E00630068006F0072002E0064006C006C0000000000340008000100500072006F006400750063007400560065007200730069006F006E00000030002E0030002E0030002E003000000038000800010041007300730065006D0062006C0079002000560065007200730069006F006E00000030002E0030002E0030002E00300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002000000C000000D03500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
    WITH PERMISSION_SET = SAFE;
    EXEC('
    CREATE FUNCTION dbo.MD5(@binaryData AS varbinary(max))
    RETURNS varbinary(16) AS EXTERNAL NAME Anchor.Utilities.HashMD5
    ');
END");

            SqlModel.AddObjects(@"CREATE FUNCTION[dbo].[MD5] (@binaryData[VARBINARY](MAX))
            RETURNS[VARBINARY] (16) WITH EXECUTE AS CALLER
            AS
                EXTERNAL NAME[Anchor].[Utilities].[HashMD5]
            GO");

            var parser = new TsqlParser();
            string finalParsedScript;
            foreach (var knot in Knot)
                SqlModel.AddObjects(knot.CreateTableStatement);
            foreach (var anchor in Anchor)
            {
                SqlModel.AddObjects(anchor.CreateTableStatement);
                foreach (var attribute in anchor.Attribute)
                    SqlModel.AddObjects(attribute.CreateTableStatement);

                finalParsedScript = parser.GetParsedSql(anchor.CreateLatestViewStatement);
                SqlModel.AddObjects(finalParsedScript);

                finalParsedScript = parser.GetParsedSql(anchor.CreateInsertSpStatement);
                SqlModel.AddObjects(finalParsedScript);

                // TODO: FIX
                //finalParsedScript = parser.GetParsedSql(anchor.CreatePitFunctionStatement);
                //SqlModel.AddObjects(finalParsedScript);

                foreach (var attr in anchor.Attribute)
                {
                    finalParsedScript = parser.GetParsedSql(attr.InsertTriggerStatement);
                    SqlModel.AddObjects(finalParsedScript);
                    finalParsedScript = parser.GetParsedSql(attr.RewinderFunctionStatement);
                    SqlModel.AddObjects(finalParsedScript);
                    finalParsedScript = parser.GetParsedSql(attr.RestatementFinderFunctionStatement);
                    SqlModel.AddObjects(finalParsedScript);
                }
            }
            foreach (var tie in Tie)
            {
                SqlModel.AddObjects(tie.CreateTableStatement);
                finalParsedScript = parser.GetParsedSql(tie.RestatementFinderFunctionStatement);
                SqlModel.AddObjects(finalParsedScript);
            }
        }
    }
}