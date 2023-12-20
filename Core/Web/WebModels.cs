using ModularSystem.Core;
using ModularSystem.Web.Expressions;
using ModularSystem.Webql.Synthesis;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace ModularSystem.Web;

/// <summary>
/// Represents a serializable and web encodable version of the <see cref="Query{T}"/> class.
/// This class provides mechanisms to convert web-friendly serialized query data into actual expressions and vice versa.
/// </summary>
/// <remarks>
/// The purpose of this class is to facilitate the transfer of complex query data (like filtering and sorting expressions) over the web by serializing the query data into strings.
/// This serialized form can then be deserialized back to actual query expressions when needed.
/// The <see cref="ExpressionSerializer"/> is used to handle the serialization and deserialization of expressions.
/// </remarks>
[Serializable]
public class SerializableQuery
{
    /// <summary>
    /// Gets or sets the serialized filter expression for the query.
    /// </summary>
    public SerializableExpression? Filter { get; set; }

    /// <summary>
    /// Gets or sets the serialized grouping expression for the query.
    /// </summary>
    public SerializableExpression? Grouping { get; set; }

    /// <summary>
    /// Gets or sets the serialized projection expression for the query.
    /// </summary>
    public SerializableExpression? Projection { get; set; }

    /// <summary>
    /// Gets or sets the serialized ordering expression for the query.
    /// </summary>
    public SerializableExpression? Ordering { get; set; }

    /// <summary>
    /// Gets or sets the pagination information for the query.
    /// </summary>
    public PaginationIn Pagination { get; set; } = new PaginationIn();

    /// <summary>
    /// Gets or sets the order direction (ascending or descending) for the query.
    /// </summary>
    public OrderingDirection OrderingDirection { get; set; } = OrderingDirection.Ascending;

    /// <summary>
    /// Converts this object to it's JSON representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializerSingleton.Serialize(this);
    }

    /// <summary>
    /// Converts the <see cref="SerializableQuery"/> instance into a <see cref="Query{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the entity the query applies to, which must be a class.</typeparam>
    /// <returns>A <see cref="Query{T}"/> instance with the properties set according to this serialized query.</returns>
    public Query<T> ToQuery<T>() where T : class
    {
        return new Query<T>()
        {
            Filter = QueryProtocol.FromSerializable(Filter),
            Ordering = QueryProtocol.FromSerializable(Ordering),
            Pagination = Pagination,
        };
    }

    static Expression? Deserialize(string? serializedExpression)
    {
        if (serializedExpression == null)
        {
            return null;
        }

        return QueryProtocol.FromJson(serializedExpression);
    }
}

/// <summary>
/// Represents a serializable and web encodable version of the <see cref="Update{T}"/> class.
/// This class provides mechanisms to convert web-friendly serialized update data into actual expressions and vice versa.
/// </summary>
/// <remarks>
/// The purpose of this class is to facilitate the transfer of complex update data (like filtering and modification expressions) over the web by serializing the update data into strings.
/// This serialized form can then be deserialized back to actual update expressions when needed.
/// The <see cref="QueryProtocol.ExpressionSerializer"/> is used to handle the serialization and deserialization of expressions.
/// </remarks>
[Serializable]
public class SerializableUpdate
{
    /// <summary>
    /// Gets or sets the serialized representation of the filter expression.
    /// </summary>
    public SerializableExpression? Filter { get; set; } = null;

    /// <summary>
    /// Gets or sets the serialized representations of the modification expressions.
    /// </summary>
    public SerializableExpression[]? Modifications { get; set; }

    /// <summary>
    /// Converts the serialized update into its corresponding <see cref="Update{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the entity being updated.</typeparam>
    /// <returns>An <see cref="Update{T}"/> object that represents the deserialized update.</returns>
    public Update<T> ToUpdate<T>() where T : class
    {
        return new Update<T>()
        {
            Filter = QueryProtocol.FromSerializable(Filter),
            Modifications = Modifications == null
                ? new()
                : Modifications.Transform(x => QueryProtocol.FromSerializable(x)).ToList(),
        };
    }

}

/// <summary>
/// Represents a builder for constructing a <see cref="WebqlQueryable"/> from a serialized query expression. <br/>
/// This class provides the functionality to convert a serialized query expression into a <see cref="WebqlQueryable"/>, enabling query execution and manipulation.
/// </summary>
public class SerializableQueryable
{
    /// <summary>
    /// Gets or sets the serialized query expression.
    /// </summary>
    public SerializableExpression? Expression { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableQueryable"/> class.
    /// </summary>
    [JsonConstructor]
    public SerializableQueryable()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableQueryable"/> class with a specified serialized expression.
    /// </summary>
    /// <param name="expression">The serialized expression to use for query construction.</param>
    public SerializableQueryable(SerializableExpression? expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Translates the serialized expression into a <see cref="WebqlQueryable"/> for a specific generic type.
    /// </summary>
    /// <param name="source">The underlying queryable object.</param>
    /// <param name="elementType">The type of the elements in the <see cref="WebqlQueryable"/>.</param>
    /// <param name="translator">An optional translator to convert expressions into a format suitable for the underlying data source (default is null).</param>
    /// <returns>A <see cref="WebqlQueryable"/> that represents the translated query.</returns>
    public WebqlQueryable ToQueryable(IQueryable source, Type elementType, Translator? translator = null)
    {
        if (Expression == null)
        {
            return new WebqlQueryable(elementType, elementType, source);
        }

        var expression = QueryProtocol.FromSerializable(Expression);

        return (translator ?? new()).TranslateToQueryable(expression, elementType, source);
    }

    /// <summary>
    /// Translates the serialized expression into a <see cref="WebqlQueryable"/> for a specific generic type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="WebqlQueryable"/>.</typeparam>
    /// <param name="source">The underlying queryable object of type <typeparamref name="T"/>.</param>
    /// <param name="translator">An optional translator to convert expressions into a format suitable for the underlying data source (default is null).</param>
    /// <returns>A <see cref="WebqlQueryable"/> that represents the translated query.</returns>
    public WebqlQueryable ToQueryable<T>(IQueryable<T> source, Translator? translator = null)
    {
        return ToQueryable(source, typeof(T), translator);
    }
}