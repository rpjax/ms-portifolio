namespace ModularSystem.Webql.Analysis.Symbols;

public enum ExpressionType
{
    Literal,
    Reference,
    Operator,
    Lambda,
    AnonymousType
}

public interface IExpressionSymbol : IStatementSymbol
{
    ExpressionType ExpressionType { get; }
}

public abstract class ExpressionSymbol : StatementSymbol, IExpressionSymbol
{
    public abstract ExpressionType ExpressionType { get; }
    public override StatementType StatementType { get; } = StatementType.Expression;
}
