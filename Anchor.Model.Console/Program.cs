using System.Linq;
using Microsoft.SqlServer.Dac.Model;

namespace Anchor.Model.Console
{
    internal class Program
    {
        private static Core.BusinessLogic.Model _model;
        private static readonly string path = "SampleModel.xml";
        public static TSqlModel SqlModel;

        private static void Main(string[] args)
        {
            var options = new TSqlModelOptions();
            SqlModel = new TSqlModel(SqlServerVersion.Sql130, options);
            _model = new Core.BusinessLogic.Model(path);

            foreach (var anch in _model.Anchor.Where(a => a.Mnemonic == "ST"))
            {
                System.Console.WriteLine(anch.CreateInsertSpStatement);

                foreach (var att in anch.Attribute)
                {
                    //System.Console.WriteLine(att.Descriptor);
                    //System.Console.WriteLine(att.Knot);
                    //System.Console.WriteLine(att.CreateTableStatement);
                }

                var res = anch.ToString();
            }

            foreach (var knot in _model.Knot)
            {
                var res = knot.ToString();
            }
            var sqlObjects = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PLV_ProfessionalLevel"));

            var clr = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction);

            string script;
            var sc = sqlObjects.TryGetScript(out script);

            _model.GenerateDacPac();
        }
    }
}