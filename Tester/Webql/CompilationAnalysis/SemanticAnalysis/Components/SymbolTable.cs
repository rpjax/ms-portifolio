using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public class SymbolTable
{
    protected internal SymbolTable? ParentTable { get; }
    private Dictionary<string, SymbolTableEntry> Entries { get; } = new();

    public SymbolTable(SymbolTable? parentTable = null)
    {
        ParentTable = parentTable;
    }

    public SymbolTable Copy()
    {
        return new SymbolTable(this);
    }

    public void AddEntry(string identifier, ISymbol symbol, Type? type, SymbolTable? symbolTable)
    {
        if (Entries.ContainsKey(identifier))
        {
            throw new Exception();
        }

        Entries.Add(identifier, new SymbolTableEntry(symbol, type, symbolTable));
    }

    public SymbolTableEntry? TryGetEntry(string identifier)
    {
        var table = this;

        while (table != null)
        {
            if (table.Entries.TryGetValue(identifier, out var value))
            {
                return value;
            }

            table = table.ParentTable;
        }

        return null;
    }

    public void UpdateEntry(string identifier, ISymbol symbol, Type? type, SymbolTable? symbolTable)
    {
        if (!ContainsEntry(identifier))
        {
            throw new Exception();
        }

        var table = this;

        while (table != null)
        {
            if (!table.Entries.TryGetValue(identifier, out var entry))
            {
                table = table.ParentTable;
                continue;
            }

            table.Entries[identifier] = new SymbolTableEntry(symbol, type, symbolTable);
            return;
        }

        throw new Exception();
    }

    //*
    //* helpers
    //*

    public bool ContainsEntry(string identifier)
    {
        return TryGetEntry(identifier) is not null;
    }

}

public class SymbolTableEntry
{
    public ISymbol Symbol { get; internal set; }
    public Type? Type { get; internal set; }
    public SymbolTable? SymbolTable { get; internal set; }

    public SymbolTableEntry(ISymbol symbol, Type? type, SymbolTable? symbolTable)
    {
        Symbol = symbol;
        Type = type;
        SymbolTable = symbolTable;
    }
}
