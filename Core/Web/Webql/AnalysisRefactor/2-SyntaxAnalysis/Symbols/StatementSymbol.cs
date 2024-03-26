namespace ModularSystem.Webql.Analysis.Symbols;

public enum StatementType
{
    Expression
}

public abstract class StatementSymbol : Symbol
{
    public abstract StatementType StatementType { get; }
}