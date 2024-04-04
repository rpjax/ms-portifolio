using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ExpressionParser : SyntaxParserBase
{
    public ExpressionSymbol ParseExpression(ParsingContext context, ObjectPropertyToken token)
    {
        return SyntaxParser.ParseOperatorExpression(context, token);
    }

    public ExpressionSymbol ParseExpression(ParsingContext context, ValueToken token)
    {
        if(token is StringToken stringToken)
        {
            if (stringToken.Value.StartsWith('$'))
            {
                return SyntaxParser.ParseReference(context, stringToken);
            }
        }
        
        return SyntaxParser.ParseLiteral(context, token);
    }

}
