using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class AttributeTableTests
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
        public void HistorizedAttributeTable()
        {
            var test = @"CREATE TABLE [dbo].[ST_NAM_Stage_Name] (
    ST_NAM_ST_ID int not null,
    ST_NAM_Stage_Name varchar(42) not null,
    ST_NAM_ChangedAt datetime not null,
    Metadata_ST_NAM int not null,
    constraint fkST_NAM_Stage_Name foreign key (
        ST_NAM_ST_ID
    ) references [dbo].[ST_Stage](ST_ID),
    constraint pkST_NAM_Stage_Name primary key (
        ST_NAM_ST_ID asc,
        ST_NAM_ChangedAt desc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_NAM_Stage_Name"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_NAM_Stage_Name"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void StaticAttributeTable()
        {
            var test = @"CREATE TABLE [dbo].[PR_NAM_Program_Name] (
    PR_NAM_PR_ID int not null,
    PR_NAM_Program_Name varchar(42) not null,
    Metadata_PR_NAM int not null,
    constraint fkPR_NAM_Program_Name foreign key (
        PR_NAM_PR_ID
    ) references [dbo].[PR_Program](PR_ID),
    constraint pkPR_NAM_Program_Name primary key (
        PR_NAM_PR_ID asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PR_NAM_Program_Name"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PR_NAM_Program_Name"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void StaticChecksumAttributeTable()
        {
            var test = @"CREATE TABLE [dbo].[ST_LOC_Stage_Location] (
    ST_LOC_ST_ID int not null,
    ST_LOC_Stage_Location geography not null,
    ST_LOC_Checksum as cast(dbo.MD5(cast(ST_LOC_Stage_Location as varbinary(max))) as varbinary(16)) persisted,
    Metadata_ST_LOC int not null,
    constraint fkST_LOC_Stage_Location foreign key (
        ST_LOC_ST_ID
    ) references [dbo].[ST_Stage](ST_ID),
    constraint pkST_LOC_Stage_Location primary key (
        ST_LOC_ST_ID asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_LOC_Stage_Location"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_LOC_Stage_Location"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void KnottedHistorizedAttributeTable()
        {
            var test = @"CREATE TABLE [dbo].[ST_AVG_Stage_Average] (
    ST_AVG_ST_ID int not null,
    ST_AVG_UTL_ID tinyint not null,
    ST_AVG_ChangedAt datetime not null,
    Metadata_ST_AVG int not null,
    constraint fk_A_ST_AVG_Stage_Average foreign key (
        ST_AVG_ST_ID
    ) references [dbo].[ST_Stage](ST_ID),
    constraint fk_K_ST_AVG_Stage_Average foreign key (
        ST_AVG_UTL_ID
    ) references [dbo].[UTL_Utilization](UTL_ID),
    constraint pkST_AVG_Stage_Average primary key (
        ST_AVG_ST_ID asc,
        ST_AVG_ChangedAt desc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_AVG_Stage_Average"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_AVG_Stage_Average"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void KnottedStaticAttributeTable()
        {
            var test = @"CREATE TABLE [dbo].[ST_MIN_Stage_Minimum] (
    ST_MIN_ST_ID int not null,
    ST_MIN_UTL_ID tinyint not null,
    Metadata_ST_MIN int not null,
    constraint fk_A_ST_MIN_Stage_Minimum foreign key (
        ST_MIN_ST_ID
    ) references [dbo].[ST_Stage](ST_ID),
    constraint fk_K_ST_MIN_Stage_Minimum foreign key (
        ST_MIN_UTL_ID
    ) references [dbo].[UTL_Utilization](UTL_ID),
    constraint pkST_MIN_Stage_Minimum primary key (
        ST_MIN_ST_ID asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_MIN_Stage_Minimum"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_MIN_Stage_Minimum"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}