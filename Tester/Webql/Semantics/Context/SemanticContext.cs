﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Webql.Core;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;
using Webql.Semantics.Definitions;
using Webql.Semantics.Extensions;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Context;

public class SemanticContext
{
    public WebqlCompilationContext CompilationContext { get; }

    /*
     * Scope related properties
     */
    private Dictionary<string, ISymbol> SymbolTable { get; }
    private WebqlScopeType? ScopeType { get; set; }

    /*
     * Technical properties
     */
    private SemanticContext? ParentContext { get; }
    private CacheObject Cache { get; }

    private SemanticContext(
        WebqlCompilationContext compilationContext,
        Dictionary<string, ISymbol> symbols,
        SemanticContext? parent,
        CacheObject cache)
    {
        CompilationContext = compilationContext;
        SymbolTable = symbols.ToDictionary(x => x.Key, x => x.Value);
        ParentContext = parent;
        Cache = cache;
    }

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

    public static SemanticContext CreateRootContext(WebqlCompilationContext compilationContext)
    {
        return new SemanticContext(
            compilationContext: compilationContext,
            symbols: new Dictionary<string, ISymbol>(),
            parent: null,
            cache: new CacheObject()
        );
    }

    public SemanticContext CreateChildContext()
    {
        return new SemanticContext(
            compilationContext: CompilationContext,
            symbols: SymbolTable.ToDictionary(x => x.Key, x => x.Value),
            parent: this,
            cache: Cache
        );
    }

}
