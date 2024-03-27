using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ExpressionArrayParser
{
    public ExpressionSymbol[] ParseExpressionArray(ParsingContext context, ArrayToken token)
    {
        var args = new List<ExpressionSymbol>(token.Values.Length);

        foreach (var item in token)
        {
            args.Add(SyntaxParser.ParseExpression(context, item));
        }

        return args.ToArray();
    }
}
