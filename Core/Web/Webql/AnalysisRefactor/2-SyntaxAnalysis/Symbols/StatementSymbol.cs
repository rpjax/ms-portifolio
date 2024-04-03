namespace ModularSystem.Webql.Analysis.Symbols;

public enum StatementType
{
    Expression,
    Declaration
}

public interface IStatementSymbol : ISymbol
{
    StatementType StatementType { get; }
}

public abstract class StatementSymbol : Symbol, IStatementSymbol
{
    public abstract StatementType StatementType { get; }
}