using Aidan.Webql.Synthesis;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Aidan.Webql.Analysis;

public class SymbolOld
{
    public string Identifier { get; }
    public Expression Expression { get; }
    public bool CanWrite { get; }

    public Type Type => Expression.Type;

    public SymbolOld(string identifier, Expression expression, bool canWrite = false)
    {
        Identifier = identifier;
        Expression = expression;
        CanWrite = canWrite;
    }


    /// <summary>
    /// Determines if the type of the current context is 'void'.
    /// </summary>
    /// <returns>True if the type is void; otherwise, false.</returns>
    public bool IsVoid()
    {
        return Type == typeof(void);
    }

    /// <summary>
    /// Determines if the type of the current context is a form of IEnumerable, indicating a queryable type.
    /// </summary>
    /// <returns>True if the type is queryable; otherwise, false.</returns>
    public bool IsQueryable()
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
    public bool IsNotQueryable()
    {
        return !IsQueryable();
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
    public Type? TryGetElementType()
    {
        if (IsNotQueryable())
        {
            return null;
        }

        if (Type.IsArray)
        {
            return Type.GetElementType();
        }

        var queryableInterface = Type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .FirstOrDefault();

        if (queryableInterface != null)
        {
            return queryableInterface.GetGenericArguments()[0];
        }

        var enumerableInterface = Type.GetInterfaces()
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
    public Type GetElementType(SemanticContextOld context)
    {
        var type = TryGetElementType();

        if (type == null)
        {
            throw SemanticThrowHelper.ErrorInternalUnknown(context, "The current context does not represent a queryable type or the queryable type is undefined. Ensure that the context is correctly initialized and represents a valid queryable type. This error may indicate a misalignment between the expected and actual types within the context.");
        }

        return type;
    }

    /// <summary>
    /// Retrieves the PropertyInfo for a given property name in the current context's type.
    /// This method searches the type's properties, considering the case-insensitivity of the name.
    /// </summary>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <returns>The PropertyInfo of the specified property, if found.</returns>
    /// <exception cref="Exception">Thrown if the property is not found in the current context and parent contexts.</exception>
    public PropertyInfo? GetPropertyInfo(string name)
    {
        return Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
            .FirstOrDefault();
    }

    /// <summary>
    /// Creates a new parameter expression based on the current context's type.
    /// </summary>
    /// <returns>A new <see cref="ParameterExpression"/>.</returns>
    public ParameterExpression CreateParameterExpression()
    {
        return Expression.Parameter(Type);
    }

    /// <summary>
    /// Determines the LINQ source type of the current context's associated type.
    /// </summary>
    /// <remarks>
    /// This method checks if the type associated with the current context is compatible with LINQ operations
    /// by determining if it implements either <see cref="IQueryable{T}"/> or <see cref="IEnumerable{T}"/>.
    /// The result indicates how the type should be treated in LINQ query translations.
    /// </remarks>
    /// <returns>
    /// A value from the <see cref="LinqSourceType"/> enumeration: either
    /// <see cref="LinqSourceType.Queryable"/> if the type implements <see cref="IQueryable{T}"/>,
    /// or <see cref="LinqSourceType.Enumerable"/> if the type implements <see cref="IEnumerable{T}"/>.
    /// </returns>
    /// <exception cref="TranslationException">
    /// Thrown when the associated type does not implement either <see cref="IQueryable{T}"/> or <see cref="IEnumerable{T}"/>,
    /// indicating that it cannot be used as a source in LINQ operations.
    /// </exception>
    public LinqSourceType GetLinqSourceType(TranslationContextOld context)
    {
        if (WebqlHelper.TypeIsQueryable(context, Type))
        {
            return LinqSourceType.Queryable;
        }
        if (WebqlHelper.TypeIsEnumerable(context, Type))
        {
            return LinqSourceType.Enumerable;
        }

        var message = $"The type '{Type.FullName}' is not supported for LINQ operations because it does not implement either IQueryable<T> or IEnumerable<T>.";

        throw new TranslationException(message, context);
    }

}

public class SymbolTableOld
{
    private Dictionary<string, SymbolOld> Table { get; } = new();

    public SymbolTableOld(SymbolTableOld? symbolTable = null)
    {
        if(symbolTable is not null)
        {
            Table = symbolTable.Table.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public Expression this[string identifier]
    {
        get
        {
            return Table[identifier].Expression;
        }
        set
        {
            Table[identifier] = new SymbolOld(identifier, value);
        }
    }

    public SymbolTableOld Copy()
    {
        return new SymbolTableOld(this);
    }

    public bool ContainsSymbol(string identifier)
    {
        return Table.ContainsKey(identifier);
    }

    public bool CanWriteSymbol(string identifier)
    {
        return TryGetSymbol(identifier)?.CanWrite == true;
    }

    public SymbolOld? TryGetSymbol(string identifier)
    {
        if(Table.TryGetValue(identifier, out var result))
        {
            return result;
        }

        return null;
    }

    public SymbolOld SetSymbol(string identifier, Expression expression, bool canWrite)
    {
        if (Table.TryGetValue(identifier, out var symbol))
        {
            if (!symbol.CanWrite)
            {
                throw new InvalidOperationException();
            }
        }

        symbol = new SymbolOld(identifier, expression, canWrite);
        Table[identifier] = symbol;
        return symbol;
    }

}
