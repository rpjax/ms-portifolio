using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public class SemanticContext
{
    private SymbolTable SymbolTable { get; } = new();

    public void AddSymbolSemantic(Symbol symbol, SymbolSemantic semantic)
    {
        if (SymbolTable.ContainsSemantic(symbol))
        {
            throw new Exception();
        }

        SymbolTable.AddSymbolSemantic(symbol, semantic);
    }

    public void AddSymbolDeclaration(string identifier, Symbol symbol)
    {
        SymbolTable.AddDeclaration(symbol, identifier);
    }

    public T? TryGetSemantic<T>(Symbol symbol) where T : SymbolSemantic
    {
        var semantics = SymbolTable.TryGetSemantic(symbol);

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

    public T? TryGetDeclaration<T>(string identifier) where T : DeclarationStatementSymbol
    {
        var symbol = SymbolTable.TryGetDeclaration(identifier);

        if (symbol is null)
        {
            return null;
        }
        if (symbol is not T result)
        {
            throw new InvalidOperationException();
        }

        return result;
    }

    public T GetSemantic<T>(Symbol symbol) where T : SymbolSemantic
    {
        var semantics = TryGetSemantic<T>(symbol); 

        if (semantics is null)
        {
            throw new InvalidOperationException();
        }

        return semantics;
    }

    public T GetDeclaration<T>(string identifier) where T : DeclarationStatementSymbol
    {
        var semantics = TryGetDeclaration<T>(identifier);

        if (semantics is null)
        {
            throw new InvalidOperationException();
        }

        return semantics;
    }

    public DeclarationStatementSymbol GetDeclaration(string identifier)
    {
        return GetDeclaration<DeclarationStatementSymbol>(identifier);
    }

    public DeclarationStatementSemantic GetDeclarationSemantic(string identifier)
    {
        return GetSemantic<DeclarationStatementSemantic>(
            symbol: GetDeclaration<DeclarationStatementSymbol>(identifier)
        );
    }
}
