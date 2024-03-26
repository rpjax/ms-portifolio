namespace ModularSystem.Webql.Analysis.Semantics;

public class SymbolTable
{
    private Dictionary<string, SymbolSemantic> SemanticTable { get; } = new();
    private Dictionary<string, Symbol> DeclarationTable { get; } = new();

    public SymbolTable(SymbolTable? symbolTable = null)
    {
        if (symbolTable is not null)
        {
            SemanticTable = symbolTable.SemanticTable.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public SymbolTable Copy()
    {
        return new SymbolTable(this);
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

public class SymbolTableRecord
{
    public Symbol Symbol { get; internal set; }
    public SymbolSemantic? Semantic { get; internal set; }

    public SymbolTableRecord(Symbol symbol, SymbolSemantic? semantic)
    {
        Symbol = symbol;
        Semantic = semantic;
    }
}
