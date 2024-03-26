namespace ModularSystem.Webql.Analysis.Symbols;

public class UnaryArgumentsSymbol : Symbol
{
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol Operand { get; }

    public UnaryArgumentsSymbol(DestinationSymbol destination, ExpressionSymbol expression)
    {
        Destination = destination;
        Operand = expression;
    }

    public override string ToString()
    {
        return $"({Destination}, {Operand})";
    }
}
