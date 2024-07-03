using System.Diagnostics.CodeAnalysis;
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
    public const string LeftHandSideId = "<lhs>";
        
    public WebqlCompilationContext CompilationContext { get; }

    private Dictionary<string, ISymbol> SymbolTable { get; }
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

    public SemanticContext CreateSubContext()
    {
        return new SemanticContext(
            compilationContext: CompilationContext,
            symbols: SymbolTable.ToDictionary(x => x.Key, x => x.Value),
            parent: this,
            cache: Cache
        );
    }

    /*
     * Symbol table helper methods
     */

    public bool ContainsSymbol(string identifier)
    {
        return SymbolTable.ContainsKey(identifier)
            || ParentContext?.ContainsSymbol(identifier) == true;
    }

    public ISymbol? TryGetSymbol(string identifier)
    {
        identifier = IdentifierHelper.NormalizeIdentifier(identifier);

        if (SymbolTable.TryGetValue(identifier, out var symbol))
        {
            return symbol;
        }

        return ParentContext?.TryGetSymbol(identifier);
    }

    public ISymbol GetSymbol(string identifier)
    {
        return TryGetSymbol(identifier) ?? throw new InvalidOperationException("Symbol not found");
    }

    public Type? TryGetSymbolType(string identifier)
    {
        return TryGetSymbol(identifier)?.Type;
    }

    public Type GetSymbolType(string identifier)
    {
        return GetSymbol(identifier).Type;
    }

    public ISemantics GetSemantics(WebqlSyntaxNode node)
    {
        return TryGetCachedSemantics(node)
            ?? ParentContext?.TryGetCachedSemantics(node) 
            ?? SemanticAnalyzer.CreateSemantics(CompilationContext, node);
    }

    public TSemantics GetSemantics<TSemantics>(WebqlSyntaxNode node) where TSemantics : ISemantics
    {
        var semantics = GetSemantics(node);

        if (semantics is TSemantics value)
        {
            return value;
        }

        throw new InvalidCastException($"Invalid semantics type: {semantics.GetType().Name}");
    }

    public void AddSymbol(ISymbol symbol)
    {
        SymbolTable.Add(IdentifierHelper.NormalizeIdentifier(symbol.Identifier), symbol);
    }

    /*
     * lhs helper methods
     */

    public LhsSymbol GetLeftHandSideSymbol()
    {
        return SymbolTable.ContainsKey(LeftHandSideId)
            ? (LhsSymbol)SymbolTable[LeftHandSideId]
            : ParentContext?.GetLeftHandSideSymbol() ?? throw new InvalidOperationException();
    }

    public Type GetLeftHandSideType()
    {
        return GetLeftHandSideSymbol().Type;
    }

    public void SetLeftHandSideSymbol(Type type)
    {
        SymbolTable[LeftHandSideId] = new LhsSymbol(LeftHandSideId, type);
    }

    /*
     * Accumulator Symbol helper methods.
     */

    public AccumulatorSymbol GetAccumulatorSymbol()
    {   
        return SymbolTable.ContainsKey(AstHelper.AccumulatorIdentifier)
            ? (AccumulatorSymbol)SymbolTable[AstHelper.AccumulatorIdentifier]
            : ParentContext?.GetAccumulatorSymbol() ?? throw new InvalidOperationException();
    }

    public Type GetAccumulatorType()
    {   
        return GetAccumulatorSymbol().Type;
    }

    public void SetAccumulatorSymbol(Type type)
    {   
        SymbolTable[AstHelper.AccumulatorIdentifier] = new AccumulatorSymbol(
            identifier: AstHelper.AccumulatorIdentifier, 
            type: type
        );
    }

    /*
     * private helper methods
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ISemantics? TryGetCachedSemantics(WebqlSyntaxNode node)
    {
        if (Cache.TryGetEntry(node.GetSemanticIdentifier(), out ISemantics? semantics))
        {
            return semantics;
        }

        return null;
    }

}
