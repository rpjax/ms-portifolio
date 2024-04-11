namespace ModularSystem.Webql.Analysis.Semantics;

public class StatementBlockSemantic : SymbolSemantic
{
    public Type ReturnType { get; }

    public StatementBlockSemantic(Type resolvedType)
    {
        ReturnType = resolvedType;
    }
}
