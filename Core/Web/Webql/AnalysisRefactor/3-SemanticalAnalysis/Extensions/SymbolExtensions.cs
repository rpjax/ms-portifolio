namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class SymbolExtensions
{
    public static void AddSemantic(this Symbol symbol, SemanticContext context, SymbolSemantic semantics)
    {
        context.AddSymbolSemantic(symbol, semantics);
    }

    public static void AddDeclaration(this Symbol symbol, SemanticContext context, string identifier)
    {
        context.AddSymbolDeclaration(identifier, symbol);
    }

    public static T? TryGetSemantic<T>(this Symbol symbol, SemanticContext context) where T : SymbolSemantic
    {
        return context.TryGetSemantic<T>(symbol);
    }

    public static T GetSemantic<T>(this Symbol symbol, SemanticContext context) where T : SymbolSemantic
    {
        return context.GetSemantic<T>(symbol);
    }

    public static T As<T>(this Symbol symbol, SemanticContext context) where T : Symbol
    {
        if(symbol is not T result)
        {
            throw new Exception();
        }

        return result;
    }

}
