namespace ModularSystem.Webql.Analysis.Semantics;

public class StatementBlockSemantic : SymbolSemantic
{
    public Type? ResolvedType { get; }

    public StatementBlockSemantic(Type? resolvedType)
    {
        ResolvedType = resolvedType;
    }
}


