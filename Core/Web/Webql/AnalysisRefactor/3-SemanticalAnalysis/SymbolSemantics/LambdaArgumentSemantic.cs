namespace ModularSystem.Webql.Analysis.Semantics;

public class LambdaArgumentSemantic : SymbolSemantic
{
    public Type Type { get; }

    public LambdaArgumentSemantic(Type type)
    {
        Type = type;
    }
}


