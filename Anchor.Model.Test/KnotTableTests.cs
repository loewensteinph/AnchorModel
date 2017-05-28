using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class KnotTableTests
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
        public void KnotCheckSum()
        {
            var test = @"CREATE TABLE [dbo].[PAT_ParentalType] (
    PAT_ID tinyint not null,
    PAT_ParentalType varchar(42) not null,
    Metadata_PAT int not null,
    constraint pkPAT_ParentalType primary key (
        PAT_ID asc
    ),
    constraint uqPAT_ParentalType unique (
        PAT_ParentalType
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PAT_ParentalType"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PAT_ParentalType"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void Knot()
        {
            var test = @"CREATE TABLE [dbo].[PLV_ProfessionalLevel] (
    PLV_ID tinyint not null,
    PLV_ProfessionalLevel varchar(max) not null,
    PLV_Checksum as cast(dbo.MD5(cast(PLV_ProfessionalLevel as varbinary(max))) as varbinary(16)) persisted,
    Metadata_PLV int not null,
    constraint pkPLV_ProfessionalLevel primary key (
        PLV_ID asc
    ),
    constraint uqPLV_ProfessionalLevel unique (
        PLV_Checksum 
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PLV_ProfessionalLevel"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PLV_ProfessionalLevel"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void Knot2()
        {
            var test = @"CREATE TABLE [dbo].[ONG_Ongoing] (
    ONG_ID tinyint not null,
    ONG_Ongoing varchar(3) not null,
    Metadata_ONG int not null,
    constraint pkONG_Ongoing primary key (
        ONG_ID asc
    ),
    constraint uqONG_Ongoing unique (
        ONG_Ongoing
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ONG_Ongoing"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ONG_Ongoing"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}