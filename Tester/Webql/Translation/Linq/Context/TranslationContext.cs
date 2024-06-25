using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing.Ast;

namespace Webql.Translation.Linq.Context;

public class TranslationContext
{
    const string LeftHandSideId = "<lhs>";

    public WebqlCompilationContext CompilationContext { get; }

    private Dictionary<string, Expression> SymbolTable { get; }
    private TranslationContext? ParentContext { get; }
    private CacheObject Cache { get; }

    private TranslationContext(
        WebqlCompilationContext compilationContext,
        Dictionary<string, Expression> symbols,
        TranslationContext? parent,
        CacheObject cache)
    {
        CompilationContext = compilationContext;
        SymbolTable = symbols.ToDictionary(x => x.Key, x => x.Value);
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
            symbols: new Dictionary<string, Expression>(),
            parent: null,
            cache: new CacheObject()
        );
    }

    public TranslationContext CreateSubContext()
    {
        return new TranslationContext(
            compilationContext: CompilationContext,
            symbols: SymbolTable.ToDictionary(x => x.Key, x => x.Value),
            parent: this,
            cache: Cache
        );
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

    public void AddExpression(string identifier, Expression expression)
    {
        if (SymbolTable.ContainsKey(identifier))
        {
            throw new InvalidOperationException($"Symbol '{identifier}' already exists.");
        }

        SymbolTable[IdentifierHelper.NormalizeIdentifier(identifier)] = expression;
    }

    /*
     * LHS expression
     */

    public void SetLeftHandSideExpression(Expression expression)
    {
        SymbolTable[LeftHandSideId] = expression;
    }

    public void SetLeftHandSideParameterExpression(Type type)
    {
        SymbolTable[LeftHandSideId] = Expression.Parameter(type, "lhs");
    }

    public Expression GetLeftHandSideExpression()
    {
        return GetExpression(LeftHandSideId);
    }

    public T GetLeftHandSideExpression<T>() where T : Expression
    {
        return GetExpression<T>(LeftHandSideId);
    }

    public Type GetLeftHandSideExpressionType()
    {
        return GetLeftHandSideExpression().Type;
    }

}
