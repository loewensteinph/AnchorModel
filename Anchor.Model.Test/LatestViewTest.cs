using System.Diagnostics;
using Anchor.Model.Core.Helper;

namespace Anchor.Model.Test
{
    [TestClass]
    public class LatestViewTest
    {
        private static Core.BusinessLogic.Model _model;
        private static readonly string path = "SampleModel.xml";

        public static TSqlModel SqlModel;

        private static TestContext _context;

        [ClassInitialize]
        public static void InitTestSuite(TestContext testContext)
        {
            _context = testContext;
            var options = new TSqlModelOptions();
            SqlModel = new TSqlModel(SqlServerVersion.Sql130, options);
            _model = new Core.BusinessLogic.Model(path);
        }

        [TestMethod]
        public void LatestViewlPE_Performance()
        {
            var test = @"CREATE VIEW [dbo].[lPE_Performance] WITH SCHEMABINDING AS
SELECT
    [PE].PE_ID,
    [PE].Metadata_PE,
    [DAT].PE_DAT_PE_ID,
    [DAT].Metadata_PE_DAT,
    [DAT].PE_DAT_Performance_Date,
    [AUD].PE_AUD_PE_ID,
    [AUD].Metadata_PE_AUD,
    [AUD].PE_AUD_Performance_Audience,
    [REV].PE_REV_PE_ID,
    [REV].Metadata_PE_REV,
    [REV].PE_REV_ChangedAt,
    [REV].PE_REV_Checksum,
    [REV].PE_REV_Performance_Revenue
FROM
    [dbo].[PE_Performance] [PE]
LEFT JOIN
    [dbo].[PE_DAT_Performance_Date] [DAT]
ON
    [DAT].PE_DAT_PE_ID = [PE].PE_ID
LEFT JOIN
    [dbo].[PE_AUD_Performance_Audience] [AUD]
ON
    [AUD].PE_AUD_PE_ID = [PE].PE_ID
LEFT JOIN
    [dbo].[PE_REV_Performance_Revenue] [REV]
ON
    [REV].PE_REV_PE_ID = [PE].PE_ID
AND
    [REV].PE_REV_ChangedAt = (
        SELECT
            max(sub.PE_REV_ChangedAt)
        FROM
            [dbo].[PE_REV_Performance_Revenue] sub
        WHERE
            sub.PE_REV_PE_ID = [PE].PE_ID
   );
GO";

            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.View)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("lPE_Performance"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.View)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("lPE_Performance"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}