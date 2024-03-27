namespace ModularSystem.Webql.Analysis.Semantics;

public class ReferenceExpressionSemantic : ExpressionSemantic
{
    public override Type Type { get; }

    public ReferenceExpressionSemantic(Type type)
    {
        Type = type;
    }
}


