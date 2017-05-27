using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Anchor.Model.Core.Helper
{
    public class TsqlParser
    {
        private readonly TSqlParser _parser = new TSql130Parser(true, SqlEngineType.All);

        public string GetParsedSql(string Script)
        {
            TSqlFragment result;
            string resultScript;
            using (TextReader sr = new StringReader(Script))
            {
                IList<ParseError> errors;
                result = _parser.Parse(sr, out errors);
            }

            SqlScriptGenerator sc = new Sql130ScriptGenerator();
            sc.GenerateScript(result, out resultScript);

            return resultScript;
        }
    }

    internal class SqlVisitor : TSqlFragmentVisitor
    {
        private int DELETEcount = 0;
        private int INSERTcount = 0;
        private int SELECTcount = 0;
        private int UPDATEcount = 0;

        private string GetNodeTokenText(TSqlFragment fragment)
        {
            var tokenText = new StringBuilder();
            for (var counter = fragment.FirstTokenIndex; counter <= fragment.LastTokenIndex; counter++)
                tokenText.Append(fragment.ScriptTokenStream[counter].Text);

            return tokenText.ToString();
        }

        public override void ExplicitVisit(CreateViewStatement node)
        {
            Visit(node);
        }
    }
}