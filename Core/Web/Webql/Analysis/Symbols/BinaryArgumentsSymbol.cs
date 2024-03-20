namespace ModularSystem.Webql.Analysis.Symbols;

public class BinaryArgumentsSymbol : Symbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol Left { get; }
    public ArgumentSymbol Right { get; }

    public BinaryArgumentsSymbol(DestinationSymbol destination, ArgumentSymbol left, ArgumentSymbol right)
    {
        Destination = destination;
        Left = left;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Destination}, {Left}, {Right})";
    }
}