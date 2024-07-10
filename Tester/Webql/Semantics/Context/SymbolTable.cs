using System.Diagnostics.CodeAnalysis;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Symbols;

public class SymbolTable
{
    private Dictionary<string, ISymbol> Entries { get; }

    public SymbolTable()
    {
        Entries = new Dictionary<string, ISymbol>();
    }

    public SymbolTable Copy()
    {
        var copy = new SymbolTable();

        foreach (var entry in Entries)
        {
            copy.AddSymbol(entry.Value);
        }

        return copy;
    }

    public bool ContainsSymbol(string identifier)
    {
        return Entries.ContainsKey(identifier);
    }

    public bool TryGetSymbol(string identifier, [MaybeNullWhen(false)] out ISymbol? symbol)
    {
        return Entries.TryGetValue(identifier, out symbol);
    }

    public ISymbol GetSymbol(string identifier)
    {
        if (!Entries.TryGetValue(identifier, out var symbol))
        {
            throw new Exception("Symbol not found");
        }

        return symbol;
    }

    public void AddSymbol(ISymbol symbol)
    {
        Entries.Add(symbol.Identifier, symbol);
    }

    public void RemoveSymbol(string identifier)
    {
        if (!Entries.ContainsKey(identifier))
        {
            throw new Exception("Symbol not found");
        }

        Entries.Remove(identifier);
    }
}
