using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public class SymbolTable
{
    private Dictionary<string, SymbolSemantic> SemanticTable { get; } = new();
    private Dictionary<string, Symbol> DeclarationTable { get; } = new();
    private Dictionary<string, SymbolTableEntry> Records { get; } = new();

    public SymbolTable(SymbolTable? symbolTable = null)
    {
        if (symbolTable is not null)
        {
            SemanticTable = symbolTable.SemanticTable.ToDictionary(x => x.Key, x => x.Value);
            DeclarationTable = symbolTable.DeclarationTable.ToDictionary(x => x.Key, x => x.Value);
            Records = symbolTable.Records.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public SymbolTable Copy()
    {
        return new SymbolTable(this);
    }

    public void AddEntry(string identifier, Symbol symbol, Type? type)
    {
        if (Records.ContainsKey(identifier))
        {
            throw new Exception();
        }

        Records.Add(identifier, new SymbolTableEntry(symbol, type, null));
    }

    public SymbolTableEntry? TryGetEntry(string identifier)
    {
        if (Records.TryGetValue(identifier, out var value))
        {
            return value;
        }

        return null;
    }

    public void UpdateEntry(string identifier, Symbol symbol, Type? type, SymbolTable? symbolTable)
    {
        if (!Records.ContainsKey(identifier))
        {
            throw new Exception();
        }

        Records[identifier] = new SymbolTableEntry(symbol, type, symbolTable);
    }

    public void AddSymbolSemantic(Symbol symbol, SymbolSemantic semantic)
    {
        if (ContainsSemantic(symbol))
        {
            throw new Exception();
        }

        SemanticTable.Add(symbol.Hash, semantic);
    }

    public void AddDeclaration(
        Symbol symbol,
        string identifier
    )
    {
        if (ContainsDeclaration(identifier))
        {
            throw new Exception();
        }

        DeclarationTable.Add(identifier, symbol);
    }

    public void UpdateDeclaration(
        Symbol symbol,
        string identifier
    )
    {
        if (!ContainsDeclaration(identifier))
        {
            throw new Exception();
        }

        DeclarationTable[identifier] = symbol;
    }

    public SymbolSemantic? TryGetSemantic(Symbol symbol)
    {
        if (SemanticTable.TryGetValue(symbol.Hash, out var value))
        {
            return value;
        }

        return null;
    }

    public Symbol? TryGetDeclaration(string identifier)
    {
        if (DeclarationTable.TryGetValue(identifier, out var value))
        {
            return value;
        }

        return null;
    }

    //*
    //* helpers
    //*

    public bool ContainsSemantic(Symbol symbol)
    {
        return SemanticTable.ContainsKey(symbol.Hash);
    }

    public bool ContainsDeclaration(string identifier)
    {
        return DeclarationTable.ContainsKey(identifier);
    }

}

public class SymbolTableEntry
{
    public Symbol Symbol { get; internal set; }
    public Type? Type { get; internal set; }
    public SymbolTable? SymbolTable { get; internal set; }

    public SymbolTableEntry(Symbol symbol, Type? type, SymbolTable? symbolTable)
    {
        Symbol = symbol;
        Type = type;
        SymbolTable = symbolTable;
    }
}
