using Microsoft.VisualBasic.FileIO;
using System.Linq.Expressions;

namespace ModularSystem.Core;

//*
// intermediary components of query object
//*

public class OrderingExpression<T> : Expression
{
    public Type FieldType { get; }
    public Expression FieldSelector { get; }
    public string FieldName { get; } 

    public OrderingExpression(Type fieldType, Expression fieldSelector)
    {
        FieldType = fieldType;
        FieldSelector = fieldSelector;
        FieldName = FieldType.FullName ?? FieldType.Name;
    }
}
