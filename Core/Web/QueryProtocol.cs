using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ModularSystem.Core;
using ModularSystem.Web.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Web;

/// <summary>
/// Defines the communication protocol for library components within a web context.<br/>
/// This protocol encapsulates the rules and configurations that guide the serialization and deserialization <br/>
/// of expressions, ensuring consistent and standardized interactions.
/// </summary>
public static class QueryProtocol
{
    /// <summary>
    /// Gets or sets the serializer responsible for converting expressions to and from their string representations.
    /// </summary>
    public static ExprSerializer ExpressionSerializer { get; set; } = DefaultExpressionSerializer();

    [return: NotNullIfNotNull("expression")]
    public static SerializableExpression? ToSerializable(Expression? expression)
    {
        if (expression == null)
        {
            return null;
        }

        return ExpressionSerializer.ToSerializable(expression);
    }

    [return: NotNullIfNotNull("sExpression")]
    public static Expression? FromSerializable(SerializableExpression? sExpression)
    {
        if (sExpression == null)
        {
            return null;
        }

        return ExpressionSerializer.FromSerializable(sExpression);
    }

    /// <summary>
    /// Converts the provided expression into its JSON string representation.
    /// </summary>
    /// <param name="expression">The expression to be serialized to JSON.</param>
    /// <param name="serializer">An optional serializer to use for the conversion. If not provided, the default <see cref="ExpressionSerializer"/> will be used.</param>
    /// <returns>The JSON string representation of the expression, or null if the expression is null.</returns>
    [return: NotNullIfNotNull("expression")]
    public static string? ToJson(Expression? expression, ISerializer<Expression>? serializer = null)
    {
        serializer ??= ExpressionSerializer;

        if (expression == null)
        {
            return null;
        }

        return serializer.Serialize(expression);
    }

    [return: NotNullIfNotNull("json")]
    public static Expression? FromJson(string? json, ISerializer<Expression>? serializer = null)
    {
        serializer ??= ExpressionSerializer;

        if (json == null)
        {
            return null;
        }

        return serializer.Deserialize(json);
    }

    /// <summary>
    /// Provides the default configuration for the expression serializer.
    /// </summary>
    /// <returns>A new instance of <see cref="ExpressionSerializer"/> with default configurations.</returns>
    static ExprSerializer DefaultExpressionSerializer()
    {
        return new ExprSerializer();
    }

}
