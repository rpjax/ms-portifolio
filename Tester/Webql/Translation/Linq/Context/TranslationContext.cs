using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing.Ast;

namespace Webql.Translation.Linq.Context;

public class TranslationContext
{
    public WebqlCompilationContext CompilationContext { get; }

    private Dictionary<string, Expression> SymbolTable { get; }
    private TranslationContext? ParentContext { get; }
    private CacheObject Cache { get; }

    internal TranslationContext(
        WebqlCompilationContext compilationContext,
        TranslationContext? parent,
        CacheObject cache)
    {
        CompilationContext = compilationContext;
        SymbolTable = new Dictionary<string, Expression>();
        ParentContext = parent;
        Cache = cache;
    }

    public class CacheObject
    {

    }

    public static TranslationContext CreateRootContext(WebqlCompilationContext compilationContext)
    {
        return new TranslationContext(
            compilationContext: compilationContext,
            parent: null,
            cache: new()
        );
    }

    public TranslationContext CreateChildContext()
    {
        return new TranslationContext(
            compilationContext: CompilationContext,
            parent: this,
            cache: Cache
        );
    }

    public bool ContainsExpression(string identifier)
    {
        if (SymbolTable.ContainsKey(identifier))
        {
            return true;
        }

        if (ParentContext is not null)
        {
            return ParentContext.ContainsExpression(identifier);
        }

        return false;
    }

    public Expression GetExpression(string identifier)
    {
        if (SymbolTable.TryGetValue(identifier, out var symbol))
        {
            return symbol;
        }

        if (ParentContext is null)
        {
            throw new InvalidOperationException($"Symbol '{identifier}' not found.");
        }

        return ParentContext.GetExpression(identifier);
    }

    public T GetExpression<T>(string identifier) where T : Expression
    {
        var symbol = GetExpression(identifier);

        if (symbol is T typedSymbol)
        {
            return typedSymbol;
        }

        throw new InvalidOperationException($"Symbol '{identifier}' is not of type '{typeof(T).Name}'.");
    }

    public void DeclareExpression(string identifier, Expression expression)
    {
        if (SymbolTable.ContainsKey(identifier))
        {
            throw new InvalidOperationException($"Symbol '{identifier}' already exists.");
        }

        SymbolTable[IdentifierHelper.NormalizeIdentifier(identifier)] = expression;
    }

    /*
     * Source parameter-expression related methods
     */

    public void DeclareSourceParameterExpression(Type type)
    {
        var parameterExpression = Expression.Parameter(
            type: type, 
            name: WebqlAstSymbols.SourceIdentifier
        );

        DeclareExpression(WebqlAstSymbols.SourceIdentifier, parameterExpression);
    }

    public ParameterExpression GetSourceParameterExpression()
    {
        return GetExpression<ParameterExpression>(WebqlAstSymbols.SourceIdentifier);
    }

    /*
     * Element parameter-expression related methods
     */

    public void DeclareElementParameterExpression(Type type)
    {
        var parameterExpression = Expression.Parameter(
            type: type, 
            name: WebqlAstSymbols.ElementIdentifier
        );

        DeclareExpression(WebqlAstSymbols.ElementIdentifier, parameterExpression);
    }

    public ParameterExpression GetElementParameterExpression()
    {
        return GetExpression<ParameterExpression>(WebqlAstSymbols.ElementIdentifier);
    }

}
