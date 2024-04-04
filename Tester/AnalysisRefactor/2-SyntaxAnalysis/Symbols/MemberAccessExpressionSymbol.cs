namespace ModularSystem.Webql.Analysis.Symbols;

// [ string_literal, expression, ]
// "$memberAccess": [<destination>, <member>, <expression>]
public class MemberAccessExpressionSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.MemberAccess;
    public string MemberName { get; }
    public ExpressionSymbol Operand => Operands[0];

    public MemberAccessExpressionSymbol(
        string memberName, 
        ExpressionSymbol operand
    )
    : base(operand)
    {
        MemberName = memberName;
    }

    public override string ToString()
    {
        return $"{Operand}.{MemberName}";
    }
}

//*
//*
//* aggregation expression symbols.
//*