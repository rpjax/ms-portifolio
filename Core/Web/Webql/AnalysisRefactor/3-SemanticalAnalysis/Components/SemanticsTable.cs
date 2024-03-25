namespace ModularSystem.Webql.Analysis.Semantics;

public class SemanticsTable
{
    private Dictionary<string, SymbolSemantics> Table { get; } = new();

    public SymbolSemantics? TryGetEntry(Symbol symbol)
    {
        if (Table.TryGetValue(symbol.Hash, out var value))
        {
            return value;
        }

        return null;
    }

    public void AddEntry(Symbol symbol, SymbolSemantics semantics)
    {
        Table.Add(symbol.Hash, semantics);
    }

    public bool ContainsKey(Symbol symbol)
    {
        return Table.ContainsKey(symbol.Hash);
    }
}
