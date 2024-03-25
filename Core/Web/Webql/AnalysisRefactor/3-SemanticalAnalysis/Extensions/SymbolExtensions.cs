namespace ModularSystem.Webql.Analysis.Semantics.Extensions;

public static class SymbolExtensions
{
    public static void AddSemantics(this Symbol symbol, SemanticContext context, SymbolSemantics semantics)
    {
        context.SemanticsTable.AddEntry(symbol, semantics);
    }

    public static T? TryGetSemantics<T>(this Symbol symbol, SemanticContext context) where T : SymbolSemantics
    {
        return context.TryGetSemantics<T>(symbol);
    }

    public static T GetSemantics<T>(this Symbol symbol, SemanticContext context) where T : SymbolSemantics
    {
        return context.GetSemantics<T>(symbol);
    }


}
