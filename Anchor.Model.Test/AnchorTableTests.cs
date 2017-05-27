using System.Diagnostics;

namespace Anchor.Model.Test
{
    [TestClass]
    public class AnchorTableTests
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
        public void Anchor()
        {
            var test = @"CREATE TABLE [dbo].[EV_Event] (
    EV_ID int IDENTITY(1,1) not null,
    Metadata_EV int not null, 
    constraint pkEV_Event primary key (
        EV_ID asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("EV_Event"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("EV_Event"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}