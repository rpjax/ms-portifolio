namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class SymbolSemantic
{

}

public abstract class ExpressionSemantic : SymbolSemantic
{
    public abstract Type Type { get; }
}

public class AxiomSemantic : SymbolSemantic
{
    //public Type QueryableType { get; }

    //public AxiomSemantics(Type queryableType)
    //{
    //    QueryableType = queryableType;
    //}
}

public class LambdaArgumentSemantic : SymbolSemantic
{
    public Type Type { get; }

    public LambdaArgumentSemantic(Type type)
    {
        Type = type;
    }
}

public class LambdaArgumentsSemantic : SymbolSemantic
{
    public Type[] Types { get; }

    public LambdaArgumentsSemantic(Type[] types)
    {
        Types = types;
    }
}

public class LambdaSemantic : SymbolSemantic
{
    public Type[] ParameterTypes { get; }
    public Type? ReturnType { get; }

    public LambdaSemantic(Type[] parameterTypes, Type? returnType)
    {
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}

public class StatementBlockSemantic : SymbolSemantic
{
    public Type? ResolvedType { get; }

    public StatementBlockSemantic(Type? resolvedType)
    {
        ResolvedType = resolvedType;
    }
}

public class ReferenceExpressionSemantic : ExpressionSemantic
{
    public override Type Type { get; }

    public ReferenceExpressionSemantic(Type type)
    {
        Type = type;
    }
}


