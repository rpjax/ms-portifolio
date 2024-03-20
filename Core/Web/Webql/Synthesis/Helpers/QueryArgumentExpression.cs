using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Symbols;

public class QueryArgumentExpression : Expression
{
    public Expression Expression { get; }
    public Type Type => Expression.Type;

    public QueryArgumentExpression(Expression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Determines if the type of the current context is a form of IEnumerable, indicating a queryable type.
    /// </summary>
    /// <returns>True if the type is queryable; otherwise, false.</returns>
    public bool IsQueryable(TranslationContext context)
    {
        return
            typeof(IEnumerable).IsAssignableFrom(Type)
            || Type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Determines if the type of the current context is NOT a form of IEnumerable, indicating a non queryable type.
    /// </summary>
    /// <returns>True if the type is queryable; otherwise, false.</returns>
    public bool IsNotQueryable(TranslationContext context)
    {
        return !IsQueryable(context);
    }

    /// <summary>
    /// Attempts to retrieve the element type of the queryable type represented by the current context. <br/>
    /// This method determines if the context type is queryable and, if so, extracts the relevant element type.
    /// </summary>
    /// <returns>
    /// The element type of the queryable type if the context is queryable; otherwise, null. 
    /// </returns>
    /// <remarks>
    /// This method checks if the context's type is either an array or implements IEnumerable{T}. <br/>
    /// If the type is an array, it returns the array's element type. <br/>
    /// If the type is a generic IEnumerable, it returns the generic argument type. <br/>
    /// If the context's type is not queryable, it returns null.
    /// </remarks>
    public Type? TryGetElementType(TranslationContext context)
    {
        var type = Expression.Type;

        if (IsNotQueryable(context))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var queryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .FirstOrDefault();

        if (queryableInterface != null)
        {
            return queryableInterface.GetGenericArguments()[0];
        }

        var enumerableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .FirstOrDefault();

        if (enumerableInterface != null)
        {
            return enumerableInterface.GetGenericArguments()[0];
        }

        return null;
    }

    /// <summary>
    /// Retrieves the element type of the queryable type of the current context, if applicable.
    /// </summary>
    /// <returns>The element type of the queryable.</returns>
    /// <exception cref="SemanticException">Thrown if the context is not queryable or the queryable type is undefined.</exception>
    public Type GetElementType(TranslationContext context)
    {
        var type = TryGetElementType(context);

        if (type == null)
        {
            throw SemanticThrowHelper.ErrorInternalUnknown(context, "The current context does not represent a queryable type or the queryable type is undefined. Ensure that the context is correctly initialized and represents a valid queryable type. This error may indicate a misalignment between the expected and actual types within the context.");
        }

        return type;
    }

    public LinqSourceType GetLinqSourceType(TranslationContext context)
    {
        return WebqlHelper.GetLinqSourceType(context, Type);
    }

}
