using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class TypeProjectionExpressionSymbol : ExpressionSymbol, IEnumerable<ProjectionBindingSymbol>
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.TypeProjection;
    public ProjectionBindingSymbol[] Bindings { get; }

    public TypeProjectionExpressionSymbol(ProjectionBindingSymbol[] expressions)
    {
        Bindings = expressions;
    }

    public IEnumerator<ProjectionBindingSymbol> GetEnumerator()
    {
        return Bindings.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Bindings.GetEnumerator();
    }

    public override string ToString()
    {
        var exprs = string.Join(Environment.NewLine, Bindings.Select(x => x.ToString() + ";"));

        return $"{{ {exprs} }}";
    }

}

public class ProjectionBindingSymbol : Symbol
{
    public string Key { get; }
    public ExpressionSymbol Value { get; }

    public ProjectionBindingSymbol(string key, ExpressionSymbol value)
    {
        Key = key;
        Value = value;      
    }

    public override string ToString()
    {
        return $"{{ {Key}: {Value} }}";
    }
}

//public class TypeBindingValueSymbol : Symbol
//{
//    public enum ValueType
//    {
//        ProjectionObject,
//        Expression
//    }

//    public ValueType Type { get; }
//    private TypeProjectionExpressionSymbol? ProjectionObject { get; }
//    private ExpressionSymbol? Expression { get; }

//    public TypeBindingValueSymbol(ExpressionSymbol expression)
//    {
//        Type = ValueType.Expression;
//        Expression = expression;
//    }

//    public TypeBindingValueSymbol(TypeProjectionExpressionSymbol typeExpression)
//    {
//        Type = ValueType.ProjectionObject;
//        ProjectionObject = typeExpression;
//        Expression = typeExpression;
//    }

//    public TypeProjectionExpressionSymbol GetProjectionObject()
//    {
//        return ProjectionObject!;
//    }

//    public ExpressionSymbol GetExpression()
//    {
//        return Expression!;
//    }

//    public override string ToString()
//    {
//        switch (Type)
//        {
//            case ValueType.ProjectionObject:
//                return ProjectionObject!.ToString();
//            case ValueType.Expression:
//                return Expression!.ToString();
//            default:
//                throw new InvalidOperationException();
//        }
//    }
//}
