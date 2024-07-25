using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

/// <summary>
/// Represents an expression that defines the ordering criteria for querying data. <br/>
/// This class encapsulates the field details and the direction in which data should be ordered.
/// </summary>
public class OrderingExpression : CustomExpression
{
    /// <summary>
    /// Gets the type of the value used in the ordering.
    /// </summary>
    public override Type Type { get; }

    /// <summary>
    /// Gets the node type of this expression. Always returns <see cref="ExpressionType.MemberAccess"/>.
    /// </summary>
    public override ExpressionType NodeType { get; }

    /// <summary>
    /// Gets the name of the field used for ordering. 
    /// If the field type has a fully qualified name, it will be used; otherwise, the simple type name will be used.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the type of the field used for ordering.
    /// </summary>
    public Type FieldType { get; }

    /// <summary>
    /// Gets the expression that selects the field from the data source for ordering.
    /// </summary>
    public Expression FieldSelector { get; }

    /// <summary>
    /// Gets the ordering direction (ascending or descending).
    /// </summary>
    public OrderingDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderingExpression"/> class.
    /// </summary>
    /// <param name="fieldType">The type of the field used for ordering.</param>
    /// <param name="fieldSelector">The expression that selects the field from the data source for ordering.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    public OrderingExpression(Type fieldType, Expression fieldSelector, OrderingDirection direction)
    {
        Type = fieldType;
        NodeType = ExpressionType.MemberAccess;
        FieldType = fieldType;
        FieldName = FieldType.FullName ?? FieldType.Name;
        FieldSelector = fieldSelector;
        Direction = direction;
    }

    /// <summary>
    /// Dispatches to the specific visit method for this node type. 
    /// For <see cref="OrderingExpression"/>, this dispatches to <see cref="CustomExpressionVisitor.VisitOrdering"/>.
    /// </summary>
    /// <param name="visitor">The visitor to visit this node with.</param>
    /// <returns>The result of visiting this node.</returns>
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is CustomExpressionVisitor custom)
        {
            return custom.VisitOrdering(this);
        }

        return this;
    }
}

/// <summary>
/// Represents an expression that defines complex ordering criteria for querying data. <br/>
/// This class encapsulates multiple ordering expressions.
/// </summary>
public class ComplexOrderingExpression : CustomExpression
{
    /// <summary>
    /// Gets the type of the complex ordering expression.
    /// </summary>
    public override Type Type { get; }

    /// <summary>
    /// Gets the node type of this expression. Always returns <see cref="ExpressionType.Extension"/>.
    /// </summary>
    public override ExpressionType NodeType { get; }

    /// <summary>
    /// Gets the type of the entity being queried.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets the array of ordering expressions.
    /// </summary>
    public Expression[] Expressions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexOrderingExpression"/> class.
    /// </summary>
    /// <param name="entityType">The type of the entity being queried.</param>
    /// <param name="expressions">The array of ordering expressions.</param>
    public ComplexOrderingExpression(Type entityType, IEnumerable<Expression> expressions)
    {
        Type = entityType;
        NodeType = (ExpressionType)ExtendedExpressionType.ComplexOrdering;
        EntityType = entityType;
        Expressions = expressions.ToArray();
    }


    /// <summary>
    /// Dispatches to the specific visit method for this node type. 
    /// For <see cref="CustomExpressionVisitor"/>, this dispatches to <see cref="CustomExpressionVisitor.VisitComplexOrdering(ComplexOrderingExpression)"/>.
    /// </summary>
    /// <param name="visitor">The visitor to visit this node with.</param>
    /// <returns>The result of visiting this node.</returns>
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is CustomExpressionVisitor custom)
        {
            return custom.VisitComplexOrdering(this);
        }

        return this;
    }
}