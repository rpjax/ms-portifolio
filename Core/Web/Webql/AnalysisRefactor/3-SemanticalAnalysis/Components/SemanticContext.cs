namespace ModularSystem.Webql.Analysis.Semantics;

public class SemanticContext
{
    public SemanticsTable SemanticsTable { get; } = new();

    public T? TryGetSemantics<T>(Symbol symbol) where T : SymbolSemantics
    {
        var semantics = SemanticsTable.TryGetEntry(symbol);

        if (semantics is null)
        {
            return null;
        }
        if(semantics is not T result)
        {
            throw new InvalidOperationException();
        }

        return result;
    }

    public T GetSemantics<T>(Symbol symbol) where T : SymbolSemantics
    {
        var semantics = TryGetSemantics<T>(symbol); 

        if (semantics is null)
        {
            throw new InvalidOperationException();
        }

        return semantics;
    }
}
