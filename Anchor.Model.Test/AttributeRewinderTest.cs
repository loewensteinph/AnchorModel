using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class AttributeRewinderTest
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
        public void AttributeRewinderTest1()
        {
            var test = @"CREATE FUNCTION [dbo].[rPE_REV_Performance_Revenue] (
        @changingTimepoint datetime
    )
    RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT
        Metadata_PE_REV,
        PE_REV_PE_ID,
        PE_REV_Checksum,
        PE_REV_Performance_Revenue,
        PE_REV_ChangedAt
    FROM
        [dbo].[PE_REV_Performance_Revenue]
    WHERE
        PE_REV_ChangedAt <= @changingTimepoint;
    ";
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

        [TestMethod]
        public void AttributeRewinderTest2()
        {
            var test = @"CREATE FUNCTION [dbo].[rAC_PLV_Actor_ProfessionalLevel] (
        @changingTimepoint datetime
    )
    RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT
        Metadata_AC_PLV,
        AC_PLV_AC_ID,
        AC_PLV_PLV_ID,
        AC_PLV_ChangedAt
    FROM
        [dbo].[AC_PLV_Actor_ProfessionalLevel]
    WHERE
        AC_PLV_ChangedAt <= @changingTimepoint;
    ";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rAC_PLV_Actor_ProfessionalLevel"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rAC_PLV_Actor_ProfessionalLevel"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void AttributeRewinderTest3()
        {
            var test = @"CREATE FUNCTION [dbo].[rST_AVG_Stage_Average] (
        @changingTimepoint datetime
    )
    RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT
        Metadata_ST_AVG,
        ST_AVG_ST_ID,
        ST_AVG_UTL_ID,
        ST_AVG_ChangedAt
    FROM
        [dbo].[ST_AVG_Stage_Average]
    WHERE
        ST_AVG_ChangedAt <= @changingTimepoint;
    ";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rST_AVG_Stage_Average"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rST_AVG_Stage_Average"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}