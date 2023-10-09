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
    public static ExpressionSerializer ExpressionSerializer { get; set; } = DefaultExpressionSerializer();

    /// <summary>
    /// Converts the provided expression into its JSON string representation.
    /// </summary>
    /// <param name="expression">The expression to be serialized to JSON.</param>
    /// <param name="serializer">An optional serializer to use for the conversion. If not provided, the default <see cref="ExpressionSerializer"/> will be used.</param>
    /// <returns>The JSON string representation of the expression, or null if the expression is null.</returns>
    [return: NotNullIfNotNull("expression")]
    public static string? ToJson(Expression? expression, ExpressionSerializer? serializer = null)
    {
        serializer ??= ExpressionSerializer;

        if (expression == null)
        {
            return null;
        }

        return serializer.ToJson(expression);
    }

    /// <summary>
    /// Provides the default configuration for the expression serializer.
    /// </summary>
    /// <returns>A new instance of <see cref="ExpressionSerializer"/> with default configurations.</returns>
    static ExpressionSerializer DefaultExpressionSerializer()
    {
        return new ExpressionSerializer(DefaultExpressionSerializerConfigs());
    }

    /// <summary>
    /// Specifies the default configurations for the expression serializer.
    /// </summary>
    /// <returns>A configuration object with default settings for the expression serializer.</returns>
    static ExpressionSerializer.Configs DefaultExpressionSerializerConfigs()
    {
        return new()
        {
            TypeSerializerOptions = new()
            {
                UseAssemblyName = true,
            }
        };
    }
}
