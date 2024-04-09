namespace ModularSystem.Webql.Analysis.Semantics;

public class DeclarationStatementSemantic : SymbolSemantic
{
    public Type Type { get; }

    public DeclarationStatementSemantic(Type type)
    {
        Type = type;
    }
}
