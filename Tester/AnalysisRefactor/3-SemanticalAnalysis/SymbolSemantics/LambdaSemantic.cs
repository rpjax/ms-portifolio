namespace ModularSystem.Webql.Analysis.Semantics;

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


