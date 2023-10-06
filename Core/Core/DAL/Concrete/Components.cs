using System.Linq.Expressions;

namespace ModularSystem.Core;

//*
// intermediary components of query object
//*

/// <summary>
/// Represents an ordering expression for querying data.
/// This class encapsulates the details of a field by which data should be ordered.
/// </summary>
public class OrderingExpression<T> : Expression
{
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
    /// Initializes a new instance of the <see cref="OrderingExpression{T}"/> class.
    /// </summary>
    /// <param name="fieldType">The type of the field used for ordering.</param>
    /// <param name="fieldSelector">The expression used to select the field from the data source.</param>
    public OrderingExpression(Type fieldType, Expression fieldSelector)
    {
        FieldType = fieldType;
        FieldSelector = fieldSelector;
        FieldName = FieldType.FullName ?? FieldType.Name;
    }
}
