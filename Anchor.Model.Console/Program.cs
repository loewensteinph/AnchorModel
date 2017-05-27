using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.SqlServer.Dac.Model;
using Schema = Anchor.Model.Core.AnchorModel.Schema;

namespace Anchor.Model.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Core.BusinessLogic.Model sch = null;
            var path = "SampleModel.xml";

            var serializer = new XmlSerializer(typeof(Schema));

            var reader = new StreamReader(path);
            sch = (Core.BusinessLogic.Model) serializer.Deserialize(reader);
            reader.Close();

            sch.InitializeSchema();

            foreach (var tie in sch.Tie)
            {
                //System.Console.Write(tie.CreateTableStatement);
            }

            foreach (var anch in sch.Anchor.Where(a => a.Mnemonic == "ST"))
            {
                System.Console.WriteLine(anch.CreateInsertSpStatement);

                foreach (var att in anch.Attribute)
                {
                    System.Console.WriteLine(att.Descriptor);
                    System.Console.WriteLine(att.Knot);
                    System.Console.WriteLine(att.CreateTableStatement);
                }

                var res = anch.ToString();
            }

            foreach (var knot in sch.Knot)
            {
                var res = knot.ToString();
            }
            var sqlObjects = sch.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.Table)
                .FirstOrDefault(tab => tab.Name.Parts[1].Equals("PLV_ProfessionalLevel"));

            var clr = sch.SqlModel.GetObjects(DacQueryScopes.UserDefined, ModelSchema.ScalarFunction);

            string script;
            var sc = sqlObjects.TryGetScript(out script);

            sch.GenerateDacPac();
        }
    }
}