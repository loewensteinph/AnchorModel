using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class AttributeRestatementFinderFunctionTest
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
        public void RestatementFinderFuncTest1()
        {
            var test = @"CREATE FUNCTION [dbo].[rfST_NAM_Stage_Name] (
        @id int,
        @value varchar(42),
        @changed datetime
    )
    RETURNS tinyint AS
    BEGIN RETURN (
        CASE WHEN EXISTS (
            SELECT
                @value 
            WHERE
                @value = (
                    SELECT TOP 1
                        pre.ST_NAM_Stage_Name
                    FROM
                        [dbo].[ST_NAM_Stage_Name] pre
                    WHERE
                        pre.ST_NAM_ST_ID = @id
                    AND
                        pre.ST_NAM_ChangedAt < @changed
                    ORDER BY
                        pre.ST_NAM_ChangedAt DESC
                )
        ) OR EXISTS (
            SELECT
                @value 
            WHERE
                @value = (
                    SELECT TOP 1
                        fol.ST_NAM_Stage_Name
                    FROM
                        [dbo].[ST_NAM_Stage_Name] fol
                    WHERE
                        fol.ST_NAM_ST_ID = @id
                    AND
                        fol.ST_NAM_ChangedAt > @changed
                    ORDER BY
                        fol.ST_NAM_ChangedAt ASC
                )
        )
        THEN 1
        ELSE 0
        END
    );
    END";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rfST_NAM_Stage_Name"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rfST_NAM_Stage_Name"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}