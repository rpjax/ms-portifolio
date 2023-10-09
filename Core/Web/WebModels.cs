using ModularSystem.Core;
using ModularSystem.Web.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;

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
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the serialized grouping expression for the query.
    /// </summary>
    public string? Grouping { get; set; }

    /// <summary>
    /// Gets or sets the serialized projection expression for the query.
    /// </summary>
    public string? Projection { get; set; }

    /// <summary>
    /// Gets or sets the serialized ordering expression for the query.
    /// </summary>
    public string? Ordering { get; set; }

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
            Pagination = Pagination,
            Filter = Deserialize(Filter),
            Grouping = Deserialize(Grouping),
            Projection = Deserialize(Projection),
            Ordering = Deserialize(Ordering),
            OrderingDirection = OrderingDirection,
        };
    }

    static Expression? Deserialize(string? serializedExpression)
    {
        if(serializedExpression == null)
        {
            return null;
        }

        var node = QueryProtocol.ExpressionSerializer.FromJson(serializedExpression);
        var expression = QueryProtocol.ExpressionSerializer.FromNode(node);

        return expression;
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
    public string? Filter { get; set; } = null;

    /// <summary>
    /// Gets or sets the serialized representations of the modification expressions.
    /// </summary>
    public string[]? Modifications { get; set; }

    /// <summary>
    /// Converts the serialized update into its corresponding <see cref="Update{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of the entity being updated.</typeparam>
    /// <returns>An <see cref="Update{T}"/> object that represents the deserialized update.</returns>
    public Update<T> ToUpdate<T>() where T : class
    {
        return new Update<T>()
        {
            Filter = Deserialize(Filter),
            Modifications = Modifications == null ? new() : Modifications.Transform(x => Deserialize(x)).ToList(),
        };
    }

    /// <summary>
    /// Deserializes a serialized expression string into its corresponding <see cref="Expression"/> object.
    /// </summary>
    /// <param name="serializedExpression">The serialized expression string.</param>
    /// <returns>The deserialized <see cref="Expression"/> object, or null if the input is null.</returns>
    [return: NotNullIfNotNull("serializedExpression")]
    static Expression? Deserialize(string? serializedExpression)
    {
        if (serializedExpression == null)
        {
            return null;
        }

        var node = QueryProtocol.ExpressionSerializer.FromJson(serializedExpression);
        var expression = QueryProtocol.ExpressionSerializer.FromNode(node);

        return expression;
    }
}
