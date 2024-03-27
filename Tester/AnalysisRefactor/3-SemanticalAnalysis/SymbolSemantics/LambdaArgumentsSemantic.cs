namespace ModularSystem.Webql.Analysis.Semantics;

public class LambdaArgumentsSemantic : SymbolSemantic
{
    public Type[] Types { get; }

    public LambdaArgumentsSemantic(Type[] types)
    {
        Types = types;
    }
}


