using System.Linq.Expressions;
using Webql.Core;
using Webql.Core.Helpers;
using Webql.Parsing.Ast;

namespace Webql.Translation.Linq.Context;

/// <summary>
/// Represents the translation context for a query. 
/// <br/>
/// Each context contains a symbol table with expressions. 
/// <br/>
/// Contexts can be nested to delimit scopes.
/// <br/>
/// Symbol resolution is performed by traversing the context hierarchy. Starting from the current context, and moving up to the parent context until the symbol is found, or the root context is reached. The first symbol found with the specified identifier is returned, so symbols can be shadowed by nested declarations.
/// </summary>
public class TranslationContext
{
    /// <summary>
    /// Gets the WebqlCompilationContext associated with the translation context.
    /// </summary>
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

    /// <summary>
    /// Represents the cache object for the translation context.
    /// </summary>
    public class CacheObject
    {

    }

    /// <summary>
    /// Creates a root translation context with the specified WebqlCompilationContext.
    /// </summary>
    /// <param name="compilationContext">The WebqlCompilationContext.</param>
    /// <returns>The root TranslationContext.</returns>
    public static TranslationContext CreateRootContext(WebqlCompilationContext compilationContext)
    {
        return new TranslationContext(
            compilationContext: compilationContext,
            parent: null,
            cache: new()
        );
    }

    /// <summary>
    /// Creates a child translation context based on the current context.
    /// </summary>
    /// <returns>The child TranslationContext.</returns>
    public TranslationContext CreateChildContext()
    {
        return new TranslationContext(
            compilationContext: CompilationContext,
            parent: this,
            cache: Cache
        );
    }

    /// <summary>
    /// Checks if the translation context contains an expression with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the expression.</param>
    /// <returns>True if the expression is found, otherwise false.</returns>
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

    /// <summary>
    /// Gets the expression with the specified identifier from the translation context.
    /// </summary>
    /// <param name="identifier">The identifier of the expression.</param>
    /// <returns>The expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the symbol is not found.</exception>
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

    /// <summary>
    /// Gets the expression with the specified identifier from the translation context, casting it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the expression.</typeparam>
    /// <param name="identifier">The identifier of the expression.</param>
    /// <returns>The expression of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the symbol is not found or is not of the specified type.</exception>
    public T GetExpression<T>(string identifier) where T : Expression
    {
        var symbol = GetExpression(identifier);

        if (symbol is T typedSymbol)
        {
            return typedSymbol;
        }

        throw new InvalidOperationException($"Symbol '{identifier}' is not of type '{typeof(T).Name}'.");
    }

    /// <summary>
    /// Declares an expression with the specified identifier in the translation context.
    /// </summary>
    /// <param name="identifier">The identifier of the expression.</param>
    /// <param name="expression">The expression to declare.</param>
    /// <exception cref="InvalidOperationException">Thrown when the symbol already exists.</exception>
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

    /// <summary>
    /// Declares the source parameter expression with the specified type in the translation context.
    /// </summary>
    /// <param name="type">The type of the source parameter expression.</param>
    public void DeclareSourceParameterExpression(Type type)
    {
        var parameterExpression = Expression.Parameter(
            type: type,
            name: WebqlAstSymbols.SourceIdentifier
        );

        DeclareExpression(WebqlAstSymbols.SourceIdentifier, parameterExpression);
    }

    /// <summary>
    /// Gets the source parameter expression from the translation context.
    /// </summary>
    /// <returns>The source parameter expression.</returns>
    public ParameterExpression GetSourceParameterExpression()
    {
        return GetExpression<ParameterExpression>(WebqlAstSymbols.SourceIdentifier);
    }

    /*
     * Element parameter-expression related methods
     */

    /// <summary>
    /// Declares the element parameter expression with the specified type in the translation context.
    /// </summary>
    /// <param name="type">The type of the element parameter expression.</param>
    public void DeclareElementParameterExpression(Type type)
    {
        var parameterExpression = Expression.Parameter(
            type: type,
            name: WebqlAstSymbols.ElementIdentifier
        );

        DeclareExpression(WebqlAstSymbols.ElementIdentifier, parameterExpression);
    }

    /// <summary>
    /// Gets the element parameter expression from the translation context.
    /// </summary>
    /// <returns>The element parameter expression.</returns>
    public ParameterExpression GetElementParameterExpression()
    {
        return GetExpression<ParameterExpression>(WebqlAstSymbols.ElementIdentifier);
    }
}
