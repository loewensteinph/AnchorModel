using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anchor.Model.Test
{
    [TestClass]
    public class TieTableTests
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
        public void KnottedHistorizedTieTable()
        {
            var test = @"CREATE TABLE [dbo].[AC_exclusive_AC_with_ONG_currently] (
    AC_ID_exclusive int not null, 
    AC_ID_with int not null, 
    ONG_ID_currently tinyint not null,
    AC_exclusive_AC_with_ONG_currently_ChangedAt datetime not null,
    Metadata_AC_exclusive_AC_with_ONG_currently int not null,
    constraint AC_exclusive_AC_with_ONG_currently_fkAC_exclusive foreign key (
        AC_ID_exclusive
    ) references [dbo].[AC_Actor](AC_ID), 
    constraint AC_exclusive_AC_with_ONG_currently_fkAC_with foreign key (
        AC_ID_with
    ) references [dbo].[AC_Actor](AC_ID), 
    constraint AC_exclusive_AC_with_ONG_currently_fkONG_currently foreign key (
        ONG_ID_currently
    ) references [dbo].[ONG_Ongoing](ONG_ID),
    constraint AC_exclusive_AC_with_ONG_currently_uqAC_exclusive unique (
        AC_ID_exclusive,
        AC_exclusive_AC_with_ONG_currently_ChangedAt
    ),
    constraint AC_exclusive_AC_with_ONG_currently_uqAC_with unique (
        AC_ID_with,
        AC_exclusive_AC_with_ONG_currently_ChangedAt
    ),
    constraint pkAC_exclusive_AC_with_ONG_currently primary key (
        AC_ID_exclusive asc,
        AC_ID_with asc,
        ONG_ID_currently asc,
        AC_exclusive_AC_with_ONG_currently_ChangedAt desc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("AC_exclusive_AC_with_ONG_currently"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("AC_exclusive_AC_with_ONG_currently"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void StaticTieTable()
        {
            var test = @"CREATE TABLE [dbo].[PE_wasHeld_ST_at] (
    PE_ID_wasHeld int not null, 
    ST_ID_at int not null, 
    Metadata_PE_wasHeld_ST_at int not null,
    constraint PE_wasHeld_ST_at_fkPE_wasHeld foreign key (
        PE_ID_wasHeld
    ) references [dbo].[PE_Performance](PE_ID), 
    constraint PE_wasHeld_ST_at_fkST_at foreign key (
        ST_ID_at
    ) references [dbo].[ST_Stage](ST_ID), 
    constraint pkPE_wasHeld_ST_at primary key (
        PE_ID_wasHeld asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PE_wasHeld_ST_at"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PE_wasHeld_ST_at"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void HistorizedStaticTieTable()
        {
            var test = @"CREATE TABLE [dbo].[ST_at_PR_isPlaying] (
    ST_ID_at int not null, 
    PR_ID_isPlaying int not null, 
    ST_at_PR_isPlaying_ChangedAt datetime not null,
    Metadata_ST_at_PR_isPlaying int not null,
    constraint ST_at_PR_isPlaying_fkST_at foreign key (
        ST_ID_at
    ) references [dbo].[ST_Stage](ST_ID), 
    constraint ST_at_PR_isPlaying_fkPR_isPlaying foreign key (
        PR_ID_isPlaying
    ) references [dbo].[PR_Program](PR_ID), 
    constraint pkST_at_PR_isPlaying primary key (
        ST_ID_at asc,
        PR_ID_isPlaying asc,
        ST_at_PR_isPlaying_ChangedAt desc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_at_PR_isPlaying"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("ST_at_PR_isPlaying"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }

        [TestMethod]
        public void StaticTieTable2()
        {
            var test = @"CREATE TABLE [dbo].[PR_content_ST_location_PE_of] (
    PR_ID_content int not null, 
    ST_ID_location int not null, 
    PE_ID_of int not null, 
    Metadata_PR_content_ST_location_PE_of int not null,
    constraint PR_content_ST_location_PE_of_fkPR_content foreign key (
        PR_ID_content
    ) references [dbo].[PR_Program](PR_ID), 
    constraint PR_content_ST_location_PE_of_fkST_location foreign key (
        ST_ID_location
    ) references [dbo].[ST_Stage](ST_ID), 
    constraint PR_content_ST_location_PE_of_fkPE_of foreign key (
        PE_ID_of
    ) references [dbo].[PE_Performance](PE_ID), 
    constraint pkPR_content_ST_location_PE_of primary key (
        PE_ID_of asc
    )
);
GO";
            SqlModel.AddObjects(test);
            var sqlObjects = SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PR_content_ST_location_PE_of"));
            string assertScript;
            Debug.Assert(sqlObjects != null, "sqlObjects != null");
            var assertSuccess = sqlObjects.TryGetScript(out assertScript);

            var testObject = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PR_content_ST_location_PE_of"));
            string testScript;
            Debug.Assert(testObject != null, "testObject != null");
            var testSuccess = testObject.TryGetScript(out testScript);

            Assert.AreEqual(true, assertSuccess);
            Assert.AreEqual(true, testSuccess);
            Assert.AreEqual(assertScript, testScript);
        }
    }
}