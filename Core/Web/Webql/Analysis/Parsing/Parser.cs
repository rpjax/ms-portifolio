using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public abstract class Parser
{
    protected T CastToken<T>(ParsingContext context, Token token) where T : Token
    {
        if(token is not T result)
        {
            throw new ParsingException("", context);
        }

        return result;
    }
}
