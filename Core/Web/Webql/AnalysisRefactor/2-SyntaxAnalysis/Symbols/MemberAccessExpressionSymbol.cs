namespace ModularSystem.Webql.Analysis.Symbols;

// [ string_literal, expression, ]
// "$memberAccess": [<destination>, <member>, <expression>]
public class MemberAccessExpressionSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.MemberAccess;
    public StringSymbol MemberName => (StringSymbol)Operands[0];
    public ExpressionSymbol Operand => Operands[1];

    public MemberAccessExpressionSymbol(
        StringSymbol memberName, 
        ExpressionSymbol operand
    )
    : base(memberName, operand)
    {

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