using System.Linq.Expressions;

namespace ModularSystem.Core.Linq.Expressions;

/// <summary>
/// Represents an update assignment expression for modifying entities.
/// </summary>
public class UpdateModificationExpression : Expression
{
    /// <summary>
    /// Gets the selector expression for the field being modified.
    /// </summary>
    public Expression Selector { get; }

    /// <summary>
    /// Gets the new value for the field.
    /// </summary>
    public Expression Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateModificationExpression"/> class.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="value"></param>
    public UpdateModificationExpression(Expression selector, Expression value)
    {
        Selector = selector;
        Value = value;
    }
}
