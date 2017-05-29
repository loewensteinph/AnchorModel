using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class PointInTimeTest
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
        public void PointInTimeTest1()
        {
            var test = @"CREATE FUNCTION [dbo].[pST_Stage] (@changingTimepoint DATETIME2(7))
RETURNS TABLE
WITH SCHEMABINDING
AS RETURN
SELECT [ST].ST_ID,
       [ST].Metadata_ST,
       [NAM].ST_NAM_ST_ID,
       [NAM].Metadata_ST_NAM,
       [NAM].ST_NAM_ChangedAt,
       [NAM].ST_NAM_Stage_Name,
       [LOC].ST_LOC_ST_ID,
       [LOC].Metadata_ST_LOC,
       [LOC].ST_LOC_Checksum,
       [LOC].ST_LOC_Stage_Location,
       [AVG].ST_AVG_ST_ID,
       [AVG].Metadata_ST_AVG,
       [AVG].ST_AVG_ChangedAt,
       [kAVG].UTL_Utilization AS ST_AVG_UTL_Utilization,
       [kAVG].Metadata_UTL AS ST_AVG_Metadata_UTL,
       [AVG].ST_AVG_UTL_ID,
       [MIN].ST_MIN_ST_ID,
       [MIN].Metadata_ST_MIN,
       [kMIN].UTL_Utilization AS ST_MIN_UTL_Utilization,
       [kMIN].Metadata_UTL AS ST_MIN_Metadata_UTL,
       [MIN].ST_MIN_UTL_ID
FROM [dbo].[ST_Stage] [ST]
    LEFT JOIN [dbo].[rST_NAM_Stage_Name](@changingTimepoint) [NAM]
        ON [NAM].ST_NAM_ST_ID = [ST].ST_ID
           AND [NAM].ST_NAM_ChangedAt =
           (
               SELECT max(sub.ST_NAM_ChangedAt)
               FROM [dbo].[rST_NAM_Stage_Name](@changingTimepoint) sub
               WHERE sub.ST_NAM_ST_ID = [ST].ST_ID
           )
    LEFT JOIN [dbo].[ST_LOC_Stage_Location] [LOC]
        ON [LOC].ST_LOC_ST_ID = [ST].ST_ID
    LEFT JOIN [dbo].[rST_AVG_Stage_Average](@changingTimepoint) [AVG]
        ON [AVG].ST_AVG_ST_ID = [ST].ST_ID
           AND [AVG].ST_AVG_ChangedAt =
           (
               SELECT max(sub.ST_AVG_ChangedAt)
               FROM [dbo].[rST_AVG_Stage_Average](@changingTimepoint) sub
               WHERE sub.ST_AVG_ST_ID = [ST].ST_ID
           )
    LEFT JOIN [dbo].[UTL_Utilization] [kAVG]
        ON [kAVG].UTL_ID = [AVG].ST_AVG_UTL_ID
    LEFT JOIN [dbo].[ST_MIN_Stage_Minimum] [MIN]
        ON [MIN].ST_MIN_ST_ID = [ST].ST_ID
    LEFT JOIN [dbo].[UTL_Utilization] [kMIN]
        ON [kMIN].UTL_ID = [MIN].ST_MIN_UTL_ID;";

            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("pST_Stage"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("pST_Stage"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void PointInTimeTest2()
        {
            var test = @"CREATE FUNCTION [dbo].[pAC_Actor] (
    @changingTimepoint datetime2(7)
)
RETURNS TABLE WITH SCHEMABINDING AS RETURN
SELECT
    [AC].AC_ID,
    [AC].Metadata_AC,
    [NAM].AC_NAM_AC_ID,
    [NAM].Metadata_AC_NAM,
    [NAM].AC_NAM_ChangedAt,
    [NAM].AC_NAM_Actor_Name,
    [GEN].AC_GEN_AC_ID,
    [GEN].Metadata_AC_GEN,
    [kGEN].GEN_Gender AS AC_GEN_GEN_Gender,
    [kGEN].Metadata_GEN AS AC_GEN_Metadata_GEN,
    [GEN].AC_GEN_GEN_ID,
    [PLV].AC_PLV_AC_ID,
    [PLV].Metadata_AC_PLV,
    [PLV].AC_PLV_ChangedAt,
    [kPLV].PLV_Checksum AS AC_PLV_PLV_Checksum,
    [kPLV].PLV_ProfessionalLevel AS AC_PLV_PLV_ProfessionalLevel,
    [kPLV].Metadata_PLV AS AC_PLV_Metadata_PLV,
    [PLV].AC_PLV_PLV_ID
FROM
    [dbo].[AC_Actor] [AC]
LEFT JOIN
    [dbo].[rAC_NAM_Actor_Name](@changingTimepoint) [NAM]
ON
    [NAM].AC_NAM_AC_ID = [AC].AC_ID
AND
    [NAM].AC_NAM_ChangedAt = (
        SELECT
            max(sub.AC_NAM_ChangedAt)
        FROM
            [dbo].[rAC_NAM_Actor_Name](@changingTimepoint) sub
        WHERE
            sub.AC_NAM_AC_ID = [AC].AC_ID
   )
LEFT JOIN
    [dbo].[AC_GEN_Actor_Gender] [GEN]
ON
    [GEN].AC_GEN_AC_ID = [AC].AC_ID
LEFT JOIN
    [dbo].[GEN_Gender] [kGEN]
ON
    [kGEN].GEN_ID = [GEN].AC_GEN_GEN_ID
LEFT JOIN
    [dbo].[rAC_PLV_Actor_ProfessionalLevel](@changingTimepoint) [PLV]
ON
    [PLV].AC_PLV_AC_ID = [AC].AC_ID
AND
    [PLV].AC_PLV_ChangedAt = (
        SELECT
            max(sub.AC_PLV_ChangedAt)
        FROM
            [dbo].[rAC_PLV_Actor_ProfessionalLevel](@changingTimepoint) sub
        WHERE
            sub.AC_PLV_AC_ID = [AC].AC_ID
   )
LEFT JOIN
    [dbo].[PLV_ProfessionalLevel] [kPLV]
ON
    [kPLV].PLV_ID = [PLV].AC_PLV_PLV_ID;
GO";

            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("pAC_Actor"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.TableValuedFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("pAC_Actor"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}