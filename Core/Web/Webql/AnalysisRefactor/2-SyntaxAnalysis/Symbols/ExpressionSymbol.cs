namespace ModularSystem.Webql.Analysis.Symbols;

public enum ExpressionType
{
    Literal,
    Reference,
    Operator,
    Lambda,
    TypeProjection
}

public abstract class ExpressionSymbol : StatementSymbol
{
    public abstract ExpressionType ExpressionType { get; }
    public override StatementType StatementType { get; } = StatementType.Expression;
}
