using Webql.Parsing.Ast;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Scope;

public class WebqlScope
{
    public WebqlScope? ParentScope { get; }
    public WebqlScopeType Type { get; }
    public SymbolTable SymbolTable { get; }

    public WebqlScope(
        WebqlScope? parent, 
        WebqlScopeType type, 
        SymbolTable? symbolTable = null)
    {
        ParentScope = parent;
        Type = type;
        SymbolTable = symbolTable?.Copy() ?? new SymbolTable();
    }

    public WebqlScope CreateChildScope(WebqlScopeType type)
    {
        return new WebqlScope(this, type);
    }

    public bool ContainsSymbol(string identifier, bool useParentScope = true)
    {
        return SymbolTable.ContainsSymbol(identifier) || (useParentScope && ParentScope?.ContainsSymbol(identifier) == true);
    }

    public IEnumerable<ISymbol> GetSymbols()
    {
        return SymbolTable.AsEnumerable();
    }

    public void DeclareSymbol(ISymbol symbol)
    {
        if(SymbolTable.TryGetSymbol(symbol.Identifier, out _))
        {
            throw new Exception($"Symbol with identifier '{symbol.Identifier}' already exists in the current scope.");
        }

        SymbolTable.AddSymbol(symbol);
    }

    public ISymbol? ResolveSymbol(string identifier)
    {
        if (SymbolTable.TryGetSymbol(identifier, out var symbol))
        {
            return symbol;
        }

        return ParentScope?.ResolveSymbol(identifier);
    }

    public TSymbol? ResolveSymbol<TSymbol>(string identifier) where TSymbol : class, ISymbol
    {
        return ResolveSymbol(identifier) as TSymbol;
    }

    /*
     * 
     */

}
