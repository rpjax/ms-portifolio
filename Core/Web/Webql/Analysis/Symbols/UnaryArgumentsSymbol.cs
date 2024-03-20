namespace ModularSystem.Webql.Analysis.Symbols;

public class UnaryArgumentsSymbol : Symbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol Argument { get; }

    public UnaryArgumentsSymbol(DestinationSymbol destination, ArgumentSymbol argument)
    {
        Destination = destination;
        Argument = argument;
    }

    public override string ToString()
    {
        return $"({Destination}, {Argument})";
    }
}
