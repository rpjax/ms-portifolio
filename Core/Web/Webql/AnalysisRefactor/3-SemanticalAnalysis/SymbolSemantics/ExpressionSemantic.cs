namespace ModularSystem.Webql.Analysis.Semantics;

public abstract class ExpressionSemantic : SymbolSemantic
{
    public abstract Type Type { get; }
}

public abstract class OperatorExpressionSemantic : ExpressionSemantic
{

}