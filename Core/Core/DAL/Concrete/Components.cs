using System.Linq.Expressions;

namespace ModularSystem.Core;

//*
// intermediary components of query operations.
//*

/// <summary>
/// Represents an ordering expression for querying data. <br/>
/// This class encapsulates the details of a field by which data should be ordered.
/// </summary>
public class OrderingExpression : CustomExpression
{
    /// <inheritdoc/>
    public override Type Type { get; }

    /// <inheritdoc/>
    public override ExpressionType NodeType { get; }

    /// <summary>
    /// Gets the type of the field used for ordering.
    /// </summary>
    public Type FieldType { get; }

    /// <summary>
    /// Gets the expression used to select the field from the data source.
    /// </summary>
    public Expression FieldSelector { get; }

    /// <summary>
    /// Gets the name of the field used for ordering. 
    /// If the field type has a full name, it will be used; otherwise, the type name will be used.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderingExpression"/> class.
    /// </summary>
    /// <param name="fieldType">The type of the field used for ordering.</param>
    /// <param name="fieldSelector">The expression used to select the field from the data source.</param>
    public OrderingExpression(Type fieldType, Expression fieldSelector)
    {
        Type = fieldType;
        NodeType = ExpressionType.MemberAccess;
        FieldType = fieldType;
        FieldSelector = fieldSelector;
        FieldName = FieldType.FullName ?? FieldType.Name;
    }

    /// <inheritdoc/>
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if(visitor is CustomExpressionVisitor custom)
        {
            return custom.VisitOrdering(this);
        }

        return this;
    }
}

public class UpdateSetExpression : NotVisitableExpression
{
    /// <inheritdoc/>
    public override Type Type { get; }

    /// <inheritdoc/>
    public override ExpressionType NodeType { get; }

    public string FieldName { get; }
    public Type FieldType { get; }
    public dynamic? Value { get; }


    public UpdateSetExpression(string fieldName, Type type, dynamic? value)
    {
        NodeType = ExpressionType.Assign;
        Type = type;
        FieldName = fieldName;
        FieldType = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{FieldName} = {Value?.ToString()}";
    }

    /// <inheritdoc/>
    protected override Expression Accept(ExpressionVisitor visitor)
    {
        if (visitor is CustomExpressionVisitor customVisitor)
        {
            return customVisitor.VisitUpdateSet(this);
        }

        return this;
    }
}