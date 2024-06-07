using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Semantics.Components;

/*
 * Semantics
 */

public interface ISemantics
{

}

public interface ITypedSemantics : ISemantics
{

}

public interface IQuerySemantics : ISemantics
{
    ITypeSymbol Type { get; }
}

public interface IExpressionSemantics : ISemantics
{
    ITypeSymbol Type { get; }
}

/*
 * Symbols
 */

public interface ISymbol
{
    string Identifier { get; }
}

public interface ITypeSymbol : ISymbol
{
    string Name { get; }
}

public class QuerySemantics : IQuerySemantics
{
    public ITypeSymbol Type { get; }

    public QuerySemantics(ITypeSymbol type)
    {
        Type = type;
    }
}

public class ExpressionSemantics : IExpressionSemantics
{
    public ITypeSymbol Type { get; }

    public ExpressionSemantics(ITypeSymbol type)
    {
        Type = type;
    }
}

public class VoidTypeSymbol : ITypeSymbol
{
    public string Identifier { get; } = "void";
    public string Name { get; } = "void";
}

public class IntTypeSymbol : ITypeSymbol
{
    public string Identifier { get; } = "int";
    public string Name { get; } = "int";
}

public class SemanticContext
{
    /// <summary>
    /// Provides a cache for all the information produced by the semantic analysis. <br/>
    /// It's shared across all the contexts that are related to the same analysis process.
    /// </summary>
    private class CacheObject
    {
        private Dictionary<string, object> Entries { get; } = new Dictionary<string, object>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(string key, object value)
        {
            Entries.Add(key, value);
        }

        [return: NotNullIfNotNull("value")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetEntry<T>(string key, out T? value) where T : class
        {
            if (Entries.TryGetValue(key, out var obj))
            {
                value = (T)obj;
                return true;
            }

            value = default;
            return false;
        }
    }

    private Guid Id { get; }
    private Dictionary<string, ISymbol> SymbolTable { get; }
    private SemanticContext? ParentContext { get; }
    private CacheObject Cache { get; }

    private SemanticContext(
        IEnumerable<ISymbol> symbols, 
        SemanticContext? parent,
        CacheObject cache)
    {
        Id = Guid.NewGuid();
        SymbolTable = symbols.ToDictionary(s => s.Identifier);
        ParentContext = parent;
        Cache = cache;
    }

    public static SemanticContext CreateRootContext()
    {
        return new SemanticContext(
            symbols: Enumerable.Empty<ISymbol>(), 
            parent: null,
            cache: new CacheObject()
        );
    }

    public SemanticContext CreateSubContext()
    {
        return new SemanticContext(
            symbols: SymbolTable.Select(x => x.Value),
            parent: this,
            cache: Cache
        );
    }

    public bool ContainsSymbol(string id)
    {
        return SymbolTable.ContainsKey(id) || ParentContext?.ContainsSymbol(id) == true;
    }

    public ISymbol? TryGetSymbol(string id)
    {
        if (SymbolTable.TryGetValue(id, out var symbol))
        {
            return symbol;
        }

        return ParentContext?.TryGetSymbol(id);
    }

    public void AddSymbol(ISymbol symbol)
    {
        SymbolTable.Add(symbol.Identifier, symbol);
    }

    public ISemantics GetSemantics(WebqlSyntaxNode node)
    {
        return ParentContext?.TryGetCachedSemantics(node) ?? SemanticAnalyzer.CreateSemantics(node);
    }

    public TSemantics GetSemantics<TSemantics>(WebqlSyntaxNode node) where TSemantics : ISemantics
    {
        var semantics = GetSemantics(node);

        if (semantics is TSemantics value)
        {
            return value;
        }

        throw new InvalidOperationException();
    }

    /*
     * private helper methods
     */

    private ISemantics? TryGetCachedSemantics(WebqlSyntaxNode node)
    {
        if (Cache.TryGetEntry(node.GetSemanticIdentifier(), out ISemantics? semantics))
        {
            return semantics;
        }

        return null;
    }

}
