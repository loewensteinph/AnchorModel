using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Anchor.Model.Core.Helper
{
    public class TsqlParser
    {
        private readonly TSqlParser _parser = new TSql130Parser(true, SqlEngineType.All);

        public string GetParsedSql(string script)
        {
            TSqlFragment result;
            using (TextReader sr = new StringReader(script))
            {
                result = _parser.Parse(sr, out IList<ParseError> errors);
            }

            SqlScriptGenerator sc = new Sql130ScriptGenerator();
            sc.GenerateScript(result, out string resultScript);

            return resultScript;
        }
    }

    internal class SqlVisitor : TSqlFragmentVisitor
    {
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