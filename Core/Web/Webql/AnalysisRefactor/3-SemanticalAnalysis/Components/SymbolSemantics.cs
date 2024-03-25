namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class SymbolSemantics
{

}

public class AxiomSemantics : SymbolSemantics
{
    //public Type QueryableType { get; }

    //public AxiomSemantics(Type queryableType)
    //{
    //    QueryableType = queryableType;
    //}
}

public class LambdaArgumentSemantics : SymbolSemantics
{
    public Type Type { get; }

    public LambdaArgumentSemantics(Type type)
    {
        Type = type;
    }
}

public class LambdaArgumentsSemantics : SymbolSemantics
{
    public Type[] Types { get; }

    public LambdaArgumentsSemantics(Type[] types)
    {
        Types = types;
    }
}

public class LambdaSemantics : SymbolSemantics
{
    public Type[] ParameterTypes { get; }
    public Type? ReturnType { get; }

    public LambdaSemantics(Type[] parameterTypes, Type? returnType)
    {
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}

public class StatementBlockSemantics : SymbolSemantics
{
    public Type? ResolvedType { get; }

    public StatementBlockSemantics(Type? resolvedType)
    {
        ResolvedType = resolvedType;
    }
}

public class ArgumentSemantics : SymbolSemantics
{
    public Type Type { get; }

    public ArgumentSemantics(Type type)
    {
        Type = type;
    }
}
