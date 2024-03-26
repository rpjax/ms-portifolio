namespace ModularSystem.Webql.Analysis.Symbols;

public class BinaryArgumentsSymbol : Symbol
{
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol LeftOperand { get; }
    public ExpressionSymbol RightOperand { get; }

    public BinaryArgumentsSymbol(DestinationSymbol destination, ExpressionSymbol left, ExpressionSymbol right)
    {
        Destination = destination;
        LeftOperand = left;
        RightOperand = right;
    }

    public override string ToString()
    {
        return $"({Destination}, {LeftOperand}, {RightOperand})";
    }
}