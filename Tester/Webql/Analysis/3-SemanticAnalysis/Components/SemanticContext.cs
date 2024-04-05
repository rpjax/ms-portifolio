﻿using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

public class SemanticContext
{
    private SemanticContext? ParentContext { get; }
    private SymbolTable SymbolTable { get; } = new();

    public SemanticContext(SemanticContext? parentContext = null)
    {
        ParentContext = parentContext;
        SymbolTable = parentContext?.SymbolTable?.Copy() ?? new SymbolTable();
    }

    public SemanticContext CreateScopeContext()
    {
        return new SemanticContext(this);
    }

    public void AddSymbolDeclaration(string identifier, Symbol symbol, Type? type)
    {
        SymbolTable.AddEntry(identifier, symbol, type);
        SymbolTable.AddDeclaration(symbol, identifier);
    }

    public void UpdateSymbolDeclaration(string identifier, Symbol symbol)
    {
        SymbolTable.UpdateDeclaration(symbol, identifier);
    }

    public SymbolTableEntry GetSymbolTableEntry(string identifier)
    {
        var entry = SymbolTable.TryGetEntry(identifier);

        if (entry is null)
        {
            throw new Exception();
        }

        return entry;
    }

    public Type GetSymbolType(string identifier)
    {
        var entry = GetSymbolTableEntry(identifier);

        if (entry.Type is null)
        {
            throw new Exception();
        }

        return entry.Type;
    }

    public void AddSymbolSemantic(Symbol symbol, SymbolSemantic semantic)
    {
        if (SymbolTable.ContainsSemantic(symbol))
        {
            throw new Exception();
        }

        SymbolTable.AddSymbolSemantic(symbol, semantic);
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

    public Symbol GetDeclaration(string identifier)
    {
        return GetDeclaration<DeclarationStatementSymbol>(identifier);
    }

    public DeclarationStatementSemantic GetDeclarationSemantic(string identifier)
    {
        return GetSemantic<DeclarationStatementSemantic>(
            symbol: GetDeclaration(identifier)
        );
    }

}
