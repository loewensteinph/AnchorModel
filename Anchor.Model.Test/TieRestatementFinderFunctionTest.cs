using System.Diagnostics;
using System.Linq;
using Anchor.Model.Core.Helper;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class TieRestatementFinderFunctionTest
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
            var test = @"CREATE FUNCTION [dbo].[rfAC_exclusive_AC_with_ONG_currently] (
        @AC_ID_exclusive int, 
        @AC_ID_with int, 
        @ONG_ID_currently tinyint,
        @changed datetime
    )
    RETURNS tinyint AS
    BEGIN RETURN (
        SELECT
            COUNT(*)
        FROM (
            SELECT TOP 1
                pre.AC_ID_exclusive,
                pre.AC_ID_with,
                pre.ONG_ID_currently
            FROM
                [dbo].[AC_exclusive_AC_with_ONG_currently] pre
            WHERE
            (
                    pre.AC_ID_exclusive = @AC_ID_exclusive
                OR
                    pre.AC_ID_with = @AC_ID_with
            )
            AND
                pre.AC_exclusive_AC_with_ONG_currently_ChangedAt < @changed
            ORDER BY
                pre.AC_exclusive_AC_with_ONG_currently_ChangedAt DESC
            UNION
            SELECT TOP 1
                fol.AC_ID_exclusive,
                fol.AC_ID_with,
                fol.ONG_ID_currently
            FROM
                [dbo].[AC_exclusive_AC_with_ONG_currently] fol
            WHERE
            (
                    fol.AC_ID_exclusive = @AC_ID_exclusive
                OR
                    fol.AC_ID_with = @AC_ID_with
            )
            AND
                fol.AC_exclusive_AC_with_ONG_currently_ChangedAt > @changed
            ORDER BY
                fol.AC_exclusive_AC_with_ONG_currently_ChangedAt ASC
        ) s
        WHERE
            s.AC_ID_exclusive = @AC_ID_exclusive
        AND
            s.AC_ID_with = @AC_ID_with
        AND
            s.ONG_ID_currently = @ONG_ID_currently
    );
    END";
            var parser = new TsqlParser();
            var finalParsedScript = parser.GetParsedSql(test);

            SqlModel.AddObjects(finalParsedScript);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rfAC_exclusive_AC_with_ONG_currently"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("rfAC_exclusive_AC_with_ONG_currently"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}