namespace ModularSystem.Webql.Analysis.Semantics;

public class ExpressionSemantic : SymbolSemantic
{
    public Type Type { get; }

    public ExpressionSemantic(Type type)
    {
        Type = type;
    }
}

public class LiteralExpressionSemantic : ExpressionSemantic
{
    public LiteralExpressionSemantic(Type type) : base(type)
    {
    }
}

public class ReferenceExpressionSemantic : ExpressionSemantic
{
    public ReferenceExpressionSemantic(Type type) : base(type)
    {
    }
}

public class OperatorExpressionSemantic : ExpressionSemantic
{
    public OperatorExpressionSemantic(Type type) : base(type)
    {
    }
}

public class LambdaExpressionSemantic : ExpressionSemantic
{
    public Type[] ParameterTypes { get; }
    public Type? ReturnType { get; }

    public LambdaExpressionSemantic(Type[] parameterTypes, Type returnType) : base(returnType)
    {
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}

public class TypeProjectionExpressionSemantic : ExpressionSemantic
{
    public TypeProjectionExpressionSemantic(Type type) : base(type)
    {
    }
}