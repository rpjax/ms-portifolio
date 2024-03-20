using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ArgumentParser : Parser
{
    public ArgumentSymbol ParseArgument(ParsingContext context, Token token)
    {
        if(token is ValueToken valueToken)
        {
            return ParseLiteralArgument(context, valueToken);      
        }

        if (token is ObjectToken objectToken)
        {
            return ParseObjectArgument(context, objectToken);
        }

        throw new ParsingException("", context);
    }

    private ArgumentSymbol ParseLiteralArgument(ParsingContext context, Token token)
    {
        if(token is NullToken)
        {
            return new NullSymbol();
        }

        if (token is StringToken stringToken)
        {
            var value = stringToken.Value;

            if (value.StartsWith('$'))
            {
                return new ReferenceSymbol(value);
            }

            return new StringSymbol(value);
        }

        if (token is NumberToken numberToken)
        {
            return new NumberSymbol(numberToken.Value);
        }

        if(token is BoolToken boolToken)
        {
            return new BoolSymbol(boolToken.Value);
        }

        throw new ParsingException("", context);
    }

    private ArgumentSymbol ParseObjectArgument(ParsingContext context, ObjectToken objectToken)
    {
        return TokenParser.ParseObject(context, objectToken);
    }

}
