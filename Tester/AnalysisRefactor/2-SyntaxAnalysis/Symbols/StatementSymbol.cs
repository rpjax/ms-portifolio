namespace ModularSystem.Webql.Analysis.Symbols;

public enum StatementType
{
    Expression,
    Declaration
}

public abstract class StatementSymbol : Symbol
{
    public abstract StatementType StatementType { get; }
}