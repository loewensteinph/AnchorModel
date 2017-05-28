using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class AttributeTriggerTest
    {
        private static Core.BusinessLogic.Model _model;
        private const string Path = "SampleModel.xml";

        public static TSqlModel SqlModel;

        private static TestContext _context;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            _context = testContext;
            var options = new TSqlModelOptions();
            SqlModel = new TSqlModel(SqlServerVersion.Sql130, options);
            _model = new Core.BusinessLogic.Model(Path);
        }

        [TestMethod]
        public void StaticAttributeTrigger()
        {
            var test = @"CREATE TRIGGER [dbo].[it_PE_DAT_Performance_Date] ON [dbo].[PE_DAT_Performance_Date]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxVersion int;
    DECLARE @currentVersion int;
    DECLARE @PE_DAT_Performance_Date TABLE (
        PE_DAT_PE_ID int not null,
        Metadata_PE_DAT int not null,
        PE_DAT_Performance_Date datetime not null,
        PE_DAT_Version bigint not null,
        PE_DAT_StatementType char(1) not null,
        primary key(
            PE_DAT_Version,
            PE_DAT_PE_ID
        )
    );
    INSERT INTO @PE_DAT_Performance_Date
    SELECT
        i.PE_DAT_PE_ID,
        i.Metadata_PE_DAT,
        i.PE_DAT_Performance_Date,
        ROW_NUMBER() OVER (
            PARTITION BY
                i.PE_DAT_PE_ID
            ORDER BY
                (SELECT 1) ASC -- some undefined order
        ),
        'X'
    FROM
        inserted i;
    SELECT
        @maxVersion = 1,
        @currentVersion = 0
    FROM
        @PE_DAT_Performance_Date;
    WHILE (@currentVersion < @maxVersion)
    BEGIN
        SET @currentVersion = @currentVersion + 1;
        UPDATE v
        SET
            v.PE_DAT_StatementType =
                CASE
                    WHEN [DAT].PE_DAT_PE_ID is not null
                    THEN 'D' -- duplicate
                    ELSE 'N' -- new statement
                END
        FROM
            @PE_DAT_Performance_Date v
        LEFT JOIN
            [dbo].[PE_DAT_Performance_Date] [DAT]
        ON
            [DAT].PE_DAT_PE_ID = v.PE_DAT_PE_ID
        AND
            [DAT].PE_DAT_Performance_Date = v.PE_DAT_Performance_Date
        WHERE
            v.PE_DAT_Version = @currentVersion;
        INSERT INTO [dbo].[PE_DAT_Performance_Date] (
            PE_DAT_PE_ID,
            Metadata_PE_DAT,
            PE_DAT_Performance_Date
        )
        SELECT
            PE_DAT_PE_ID,
            Metadata_PE_DAT,
            PE_DAT_Performance_Date
        FROM
            @PE_DAT_Performance_Date
        WHERE
            PE_DAT_Version = @currentVersion
        AND
            PE_DAT_StatementType in ('N');
    END
END
GO";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_PE_DAT_Performance_Date"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_PE_DAT_Performance_Date"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void HistorizedAttributeTrigger()
        {
            var test = @"CREATE TRIGGER [dbo].[it_PE_REV_Performance_Revenue] ON [dbo].[PE_REV_Performance_Revenue]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxVersion int;
    DECLARE @currentVersion int;
    DECLARE @PE_REV_Performance_Revenue TABLE (
        PE_REV_PE_ID int not null,
        Metadata_PE_REV int not null,
        PE_REV_ChangedAt datetime not null,
        PE_REV_Performance_Revenue money not null,
        PE_REV_Checksum varbinary(16) not null,
        PE_REV_Version bigint not null,
        PE_REV_StatementType char(1) not null,
        primary key(
            PE_REV_Version,
            PE_REV_PE_ID
        )
    );
    INSERT INTO @PE_REV_Performance_Revenue
    SELECT
        i.PE_REV_PE_ID,
        i.Metadata_PE_REV,
        i.PE_REV_ChangedAt,
        i.PE_REV_Performance_Revenue,
        dbo.MD5(cast(i.PE_REV_Performance_Revenue as varbinary(max))),
        DENSE_RANK() OVER (
            PARTITION BY
                i.PE_REV_PE_ID
            ORDER BY
                i.PE_REV_ChangedAt ASC
        ),
        'X'
    FROM
        inserted i;
    SELECT
        @maxVersion = max(PE_REV_Version), 
        @currentVersion = 0
    FROM
        @PE_REV_Performance_Revenue;
    WHILE (@currentVersion < @maxVersion)
    BEGIN
        SET @currentVersion = @currentVersion + 1;
        UPDATE v
        SET
            v.PE_REV_StatementType =
                CASE
                    WHEN [REV].PE_REV_PE_ID is not null
                    THEN 'D' -- duplicate
                    WHEN [dbo].[rfPE_REV_Performance_Revenue](
                        v.PE_REV_PE_ID,
                        v.PE_REV_Checksum, 
                        v.PE_REV_ChangedAt
                    ) = 1
                    THEN 'R' -- restatement
                    ELSE 'N' -- new statement
                END
        FROM
            @PE_REV_Performance_Revenue v
        LEFT JOIN
            [dbo].[PE_REV_Performance_Revenue] [REV]
        ON
            [REV].PE_REV_PE_ID = v.PE_REV_PE_ID
        AND
            [REV].PE_REV_ChangedAt = v.PE_REV_ChangedAt
        AND
            [REV].PE_REV_Checksum = v.PE_REV_Checksum 
        WHERE
            v.PE_REV_Version = @currentVersion;
        INSERT INTO [dbo].[PE_REV_Performance_Revenue] (
            PE_REV_PE_ID,
            Metadata_PE_REV,
            PE_REV_ChangedAt,
            PE_REV_Performance_Revenue
        )
        SELECT
            PE_REV_PE_ID,
            Metadata_PE_REV,
            PE_REV_ChangedAt,
            PE_REV_Performance_Revenue
        FROM
            @PE_REV_Performance_Revenue
        WHERE
            PE_REV_Version = @currentVersion
        AND
            PE_REV_StatementType in ('N');
    END
END
GO";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_PE_REV_Performance_Revenue"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_PE_REV_Performance_Revenue"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void HistorizedAttributeTrigger2()
        {
            var test =
                @"CREATE TRIGGER [dbo].[it_AC_PLV_Actor_ProfessionalLevel] ON [dbo].[AC_PLV_Actor_ProfessionalLevel]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxVersion int;
    DECLARE @currentVersion int;
    DECLARE @AC_PLV_Actor_ProfessionalLevel TABLE (
        AC_PLV_AC_ID int not null,
        Metadata_AC_PLV int not null,
        AC_PLV_ChangedAt datetime not null,
        AC_PLV_PLV_ID tinyint not null, 
        AC_PLV_Version bigint not null,
        AC_PLV_StatementType char(1) not null,
        primary key(
            AC_PLV_Version,
            AC_PLV_AC_ID
        )
    );
    INSERT INTO @AC_PLV_Actor_ProfessionalLevel
    SELECT
        i.AC_PLV_AC_ID,
        i.Metadata_AC_PLV,
        i.AC_PLV_ChangedAt,
        i.AC_PLV_PLV_ID,
        DENSE_RANK() OVER (
            PARTITION BY
                i.AC_PLV_AC_ID
            ORDER BY
                i.AC_PLV_ChangedAt ASC
        ),
        'X'
    FROM
        inserted i;
    SELECT
        @maxVersion = max(AC_PLV_Version), 
        @currentVersion = 0
    FROM
        @AC_PLV_Actor_ProfessionalLevel;
    WHILE (@currentVersion < @maxVersion)
    BEGIN
        SET @currentVersion = @currentVersion + 1;
        UPDATE v
        SET
            v.AC_PLV_StatementType =
                CASE
                    WHEN [PLV].AC_PLV_AC_ID is not null
                    THEN 'D' -- duplicate
                    WHEN [dbo].[rfAC_PLV_Actor_ProfessionalLevel](
                        v.AC_PLV_AC_ID,
                        v.AC_PLV_PLV_ID,
                        v.AC_PLV_ChangedAt
                    ) = 1
                    THEN 'R' -- restatement
                    ELSE 'N' -- new statement
                END
        FROM
            @AC_PLV_Actor_ProfessionalLevel v
        LEFT JOIN
            [dbo].[AC_PLV_Actor_ProfessionalLevel] [PLV]
        ON
            [PLV].AC_PLV_AC_ID = v.AC_PLV_AC_ID
        AND
            [PLV].AC_PLV_ChangedAt = v.AC_PLV_ChangedAt
        AND
            [PLV].AC_PLV_PLV_ID = v.AC_PLV_PLV_ID
        WHERE
            v.AC_PLV_Version = @currentVersion;
        INSERT INTO [dbo].[AC_PLV_Actor_ProfessionalLevel] (
            AC_PLV_AC_ID,
            Metadata_AC_PLV,
            AC_PLV_ChangedAt,
            AC_PLV_PLV_ID
        )
        SELECT
            AC_PLV_AC_ID,
            Metadata_AC_PLV,
            AC_PLV_ChangedAt,
            AC_PLV_PLV_ID
        FROM
            @AC_PLV_Actor_ProfessionalLevel
        WHERE
            AC_PLV_Version = @currentVersion
        AND
            AC_PLV_StatementType in ('N','R');
    END
END
GO";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_AC_PLV_Actor_ProfessionalLevel"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.DmlTrigger)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("it_AC_PLV_Actor_ProfessionalLevel"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}