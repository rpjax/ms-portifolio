using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

/// <summary>
/// Represents an expression that defines a field update operation. <br/>
/// This expression is not intended to be visited by standard expression visitors.
/// </summary>
public class UpdateSetExpression : NotVisitableExpression
{
    /// <summary>
    /// Gets the type of the value being set.
    /// </summary>
    public override Type Type { get; }

    /// <summary>
    /// Gets the node type of this expression. Always returns <see cref="ExpressionType.Assign"/>.
    /// </summary>
    public override ExpressionType NodeType { get; }

    /// <summary>
    /// Gets the name of the field being updated.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the type of the field being updated.
    /// </summary>
    public Type FieldType { get; }

    /// <summary>
    /// Gets the value to set for the field.
    /// </summary>
    public dynamic? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSetExpression"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the field being updated.</param>
    /// <param name="type">The type of the value being set.</param>
    /// <param name="value">The value to set for the field.</param>
    public UpdateSetExpression(string fieldName, Type type, dynamic? value)
    {
        NodeType = (ExpressionType) ExtendedExpressionType.UpdateSet;
        Type = type;
        FieldName = fieldName;
        FieldType = type;
        Value = value;
    }

    /// <summary>
    /// Returns a string representation of the update set expression.
    /// </summary>
    /// <returns>A string in the format "FieldName = Value".</returns>
    public override string ToString()
    {
        return $"{FieldName} = {Value?.ToString()}";
    }

    /// <summary>
    /// Dispatches to the specific visit method for this node type. 
    /// For <see cref="UpdateSetExpression"/>, this dispatches to <see cref="CustomExpressionVisitor.VisitUpdateSet"/>.
    /// </summary>
    /// <param name="visitor">The visitor to visit this node with.</param>
    /// <returns>The result of visiting this node.</returns>
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is CustomExpressionVisitor customVisitor)
        {
            return customVisitor.VisitUpdateSet(this);
        }

        return this;
    }
}
