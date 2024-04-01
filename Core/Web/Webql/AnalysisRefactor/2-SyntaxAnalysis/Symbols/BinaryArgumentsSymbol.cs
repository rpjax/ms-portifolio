namespace ModularSystem.Webql.Analysis.Symbols;

public class BinaryArgumentsSymbol : Symbol
{
    public StringSymbol Destination { get; }
    public ExpressionSymbol LeftOperand { get; }
    public ExpressionSymbol RightOperand { get; }

    public BinaryArgumentsSymbol(StringSymbol destination, ExpressionSymbol left, ExpressionSymbol right)
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