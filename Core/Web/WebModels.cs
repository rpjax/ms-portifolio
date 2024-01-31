using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Linq;
using ModularSystem.Web.Expressions;
using ModularSystem.Webql.Synthesis;
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
    /// Builds an <see cref="IQueryable"/> based on the serialized expression and the given source queryable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source <see cref="IQueryable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IQueryable{T}"/> to which the serialized expression is applied.</param>
    /// <returns>An <see cref="IQueryable"/> object that represents the transformed queryable.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no expression is set or if the expression type is invalid.</exception>
    /// <exception cref="ErrorException">Thrown when the transformation of the queryable fails, with detailed error information.</exception>
    /// <remarks>
    /// The method uses the serialized expression to generate a new queryable object. It checks the validity of the expression,
    /// binds necessary parameters, and dynamically compiles and invokes a lambda expression to transform the source queryable.
    /// If the transformation process fails, detailed error information is provided.
    /// </remarks>
    public IQueryable BuildQueryable<T>(IQueryable<T> source)
    {
        if (Expression == null)
        {
            throw new InvalidOperationException();
        }

        var expression = QueryProtocol.FromSerializable(Expression);

        if (!expression.Type.IsGenericType)
        {
            throw new InvalidOperationException();
        }

        var visitor = new ParameterExpressionReferenceBinder();

        var inputType = typeof(IQueryable<T>);
        var parameter = System.Linq.Expressions.Expression.Parameter(inputType, "root");
        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = typeof(IQueryable<>).MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(inputType, outputType);

        var lambdaExpression = visitor
            .Visit(System.Linq.Expressions.Expression.Lambda(lambdaExpressionType, expression, parameter))
            .TypeCast<LambdaExpression>();

        var lambda = lambdaExpression.Compile();

        var transformedQueryable = (IQueryable?)lambda.DynamicInvoke(source);

        if (transformedQueryable == null)
        {
            var typeSerializer = new TypeSerializer();
            var message = "Failed to transform queryable.";
            var error = new Error(message)
                .AddJsonData("Source Queryable Type", typeSerializer.Serialize(inputType))
                .AddJsonData("Transformed Queryable Type", typeSerializer.Serialize(outputType))
                .AddJsonData("Projected Type", typeSerializer.Serialize(projectedType))
                .AddJsonData("Raw SerializableExpression", Expression)
                .AddFlags(ErrorFlags.Bug, ErrorFlags.Debug);

            throw new ErrorException(error);
        }

        return transformedQueryable;
    }

    /// <summary>
    /// Wraps the result of <see cref="BuildQueryable{T}"/> into a <see cref="QueryableWrapper"/>, making it an <see cref="IQueryable{object}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source <see cref="IQueryable{T}"/>.</typeparam>
    /// <param name="source">The source <see cref="IQueryable{T}"/> to apply the serialized expression to.</param>
    /// <returns>A <see cref="QueryableWrapper"/> object that encapsulates the <see cref="IQueryable"/> resulting from <see cref="BuildQueryable{T}"/>.</returns>
    /// <remarks>
    /// This method serves as a convenience wrapper around the <see cref="BuildQueryable{T}"/> method. <br/>
    /// It converts the result into a <see cref="QueryableWrapper"/>,
    /// allowing the returned queryable to be treated as an <see cref="IQueryable{object}"/>.
    /// </remarks>
    public IQueryable<object> ToQueryable<T>(IQueryable<T> source)
    {      
        return new QueryableWrapper(BuildQueryable(source));
    }

}
