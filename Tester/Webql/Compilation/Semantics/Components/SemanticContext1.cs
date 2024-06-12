using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Semantics.Components;

public class SemanticContext
{
    const string LeftHandSideKey = "<lhs>";

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
        Dictionary<string, ISymbol> symbols,
        SemanticContext? parent,
        CacheObject cache)
    {
        Id = Guid.NewGuid();
        SymbolTable = symbols.ToDictionary(x => x.Key, x => x.Value);
        ParentContext = parent;
        Cache = cache;
    }

    public static SemanticContext CreateRootContext()
    {
        return new SemanticContext(
            symbols: new Dictionary<string, ISymbol>(),
            parent: null,
            cache: new CacheObject()
        );
    }

    public SemanticContext CreateSubContext()
    {
        return new SemanticContext(
            symbols: SymbolTable.ToDictionary(x => x.Key, x => x.Value),
            parent: this,
            cache: Cache
        );
    }

    public bool ContainsSymbol(string identifier)
    {
        return SymbolTable.ContainsKey(identifier)
            || ParentContext?.ContainsSymbol(identifier) == true;
    }

    public ISymbol? TryGetSymbol(string identifier)
    {
        if (SymbolTable.TryGetValue(identifier, out var symbol))
        {
            return symbol;
        }

        return ParentContext?.TryGetSymbol(identifier);
    }

    public void AddSymbol(string identifier, ISymbol symbol)
    {
        SymbolTable.Add(identifier, symbol);
    }

    public ISemantics GetSemantics(WebqlSyntaxNode node)
    {
        return TryGetCachedSemantics(node)
            ?? ParentContext?.TryGetCachedSemantics(node) 
            ?? SemanticAnalyzer.CreateSemantics(node);
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
     * lhs helper methods
     */

    public void SetLeftHandSide(ISymbol symbol)
    {
        SymbolTable[LeftHandSideKey] = symbol;
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
