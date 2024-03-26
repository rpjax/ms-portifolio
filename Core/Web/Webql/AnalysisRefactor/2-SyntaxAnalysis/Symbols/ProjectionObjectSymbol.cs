using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class ProjectionObjectSymbol : Symbol, IEnumerable<ProjectionObjectExprSymbol>
{
    public ProjectionObjectExprSymbol[] Expressions { get; }

    public ProjectionObjectSymbol(ProjectionObjectExprSymbol[] expressions)
    {
        Expressions = expressions;
    }

    public IEnumerator<ProjectionObjectExprSymbol> GetEnumerator()
    {
        return Expressions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Expressions.GetEnumerator();
    }

    public override string ToString()
    {
        var exprs = string.Join(Environment.NewLine, Expressions.Select(x => x.ToString() + ";"));

        return $"{{ {exprs} }}";
    }

}

public class ProjectionObjectExprSymbol : Symbol
{
    public string Key { get; }
    public ProjectionObjectExprValueSymbol Value { get; }

    public ProjectionObjectExprSymbol(string key, ProjectionObjectExprValueSymbol value)
    {
        Key = key;
        Value = value;
        
    }

    public override string ToString()
    {
        return $"{{ {Key}: {Value} }}";
    }
}

public class ProjectionObjectExprValueSymbol : Symbol
{
    public enum ValueType
    {
        Reference,
        ProjectionObject,
        Expr
    }

    public ValueType Type { get; }
    private ReferenceExpressionSymbol? Reference { get; }
    private ProjectionObjectSymbol? ProjectionObject { get; }
    private OperatorExpressionSymbol? Expr { get; }

    public ProjectionObjectExprValueSymbol(ReferenceExpressionSymbol reference)
    {
        Type = ValueType.Reference;
        Reference = reference;
    }

    public ProjectionObjectExprValueSymbol(ProjectionObjectSymbol projectionObject)
    {
        Type = ValueType.ProjectionObject;
        ProjectionObject = projectionObject;
    }

    public ProjectionObjectExprValueSymbol(OperatorExpressionSymbol expr)
    {
        Type = ValueType.Expr;
        Expr = expr;
    }

    public ReferenceExpressionSymbol GetReference() => Reference!;
    public ProjectionObjectSymbol GetProjectionObject() => ProjectionObject!;
    public OperatorExpressionSymbol GetExpr() => Expr!;

    public override string ToString()
    {
        switch (Type)
        {
            case ValueType.Reference:
                return Reference!.ToString();
            case ValueType.ProjectionObject:
                return ProjectionObject!.ToString();
            case ValueType.Expr:
                return Expr!.ToString();
            default:
                throw new InvalidOperationException();
        }
    }
}

//public class ProjectionReferenceValueSymbol : ProjectionValueSymbol
//{
//    public ReferenceSymbol Reference { get; }

//    public ProjectionReferenceValueSymbol(ReferenceSymbol reference)
//    {
//        Reference = reference;
//    }

//    public override string ToString()
//    {
//        return Reference.ToString();
//    }
//}

//public class ProjectionObjectValueSymbol : ProjectionValueSymbol
//{
//    public ProjectionObjectSymbol Object { get; }

//    public ProjectionObjectValueSymbol(ProjectionObjectSymbol obj)
//    {
//        Object = obj;
//    }

//    public override string ToString()
//    {
//        return Object.ToString();
//    }
//}
