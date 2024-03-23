using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ObjectParser : Parser
{
    public ObjectSymbol ParseObject(ParsingContext context, ObjectToken token)
    {
        var exprs = new List<ExprSymbol>();

        foreach (var item in token)
        {
            exprs.Add(TokenParser.ParseExpr(context, item));
        }

        return new ObjectSymbol(exprs.ToArray());
    }
}

