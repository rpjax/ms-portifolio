using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public abstract class SerializableExpression
{
    /// <summary>
    /// Gets or sets the specific type of the node within the expression tree, like Add, Subtract, And, Or, etc.
    /// </summary>
    public ExpressionType ExpressionType { get; set; }
}

/// <summary>
/// Represents a serializable version of <see cref="ConstantExpression"/>.
/// </summary>
public class SerializableConstExpression : SerializableExpression
{
    /// <summary>
    /// Gets or sets the type of the constant.
    /// </summary>
    public SerializedType? Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the constant as a string.
    /// </summary>
    public string? Value { get; set; }
}