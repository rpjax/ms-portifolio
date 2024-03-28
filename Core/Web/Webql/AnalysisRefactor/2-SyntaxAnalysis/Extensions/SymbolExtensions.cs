using ModularSystem.Webql.Analysis.Parsing;

namespace ModularSystem.Webql.Analysis.Syntax.Extensions;

public static class SymbolExtensions
{
    public static T As<T>(this Symbol symbol, ParsingContext context) where T : Symbol
    {
        if (symbol is not T result)
        {
            throw new Exception();
        }

        return result;
    }
}
