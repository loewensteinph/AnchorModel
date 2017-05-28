using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class FinderFunctionTest
    {
        private const string Path = "SampleModel.xml";
        private static Core.BusinessLogic.Model _model;

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
        public void RestatementFinderFuncTest1()
        {
            var test = @"CREATE FUNCTION [dbo].[rPE_REV_Performance_Revenue] (@changingTimepoint DATETIME)
RETURNS TABLE
WITH SCHEMABINDING
AS RETURN
SELECT Metadata_PE_REV,
       PE_REV_PE_ID,
       PE_REV_Checksum,
       PE_REV_Performance_Revenue,
       PE_REV_ChangedAt
FROM [dbo].[PE_REV_Performance_Revenue]
WHERE PE_REV_ChangedAt <= @changingTimepoint;";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rPE_REV_Performance_Revenue"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rPE_REV_Performance_Revenue"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}