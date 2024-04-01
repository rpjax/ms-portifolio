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
        ProjectionObject,
        Expression
    }

    public ValueType Type { get; }
    private ProjectionObjectSymbol? ProjectionObject { get; }
    private ExpressionSymbol? Expression { get; }

    public ProjectionObjectExprValueSymbol(ExpressionSymbol expression)
    {
        Type = ValueType.Expression;
        Expression = expression;
    }

    public ProjectionObjectExprValueSymbol(ProjectionObjectSymbol projectionObject)
    {
        Type = ValueType.ProjectionObject;
        ProjectionObject = projectionObject;
    }

    public ProjectionObjectSymbol GetProjectionObject() => ProjectionObject!;
    public ExpressionSymbol GetExpression() => Expression!;

    public override string ToString()
    {
        switch (Type)
        {
            case ValueType.ProjectionObject:
                return ProjectionObject!.ToString();
            case ValueType.Expression:
                return Expression!.ToString();
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
