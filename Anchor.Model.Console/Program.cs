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

            foreach (var anch in _model.Anchor)
            {
                System.Console.WriteLine(anch.CreatePitFunctionStatement);

                foreach (var att in anch.Attribute)
                    //System.Console.WriteLine(att.Descriptor);
                    //System.Console.WriteLine(att.Knot);
                    System.Console.WriteLine(att.RestatementFinderFunctionStatement);

                var res = anch.ToString();
            }

            foreach (var tie in _model.Tie.Where(t => t.IsHistorized && !t.HasIdentifiers))
            {
                System.Console.WriteLine(tie.TableName);
                System.Console.WriteLine(tie.RestatementFinderFunctionStatement);
            }
            //var sqlObjects = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
            //    .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PLV_ProfessionalLevel"));

            //var clr = _model.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction);

            //string script;
            //var sc = sqlObjects.TryGetScript(out script);

            _model.GenerateDacPac();
        }
    }
}