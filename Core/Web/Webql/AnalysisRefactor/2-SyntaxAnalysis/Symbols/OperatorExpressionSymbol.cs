namespace ModularSystem.Webql.Analysis.Symbols;

public abstract class OperatorExpressionSymbol : ExpressionSymbol
{
    public abstract OperatorType Operator { get; }
    public override ExpressionType ExpressionType { get; } = ExpressionType.Operator;
    public ExpressionSymbol[] Operands { get; init; }

    public int OperandsCount => 0;

    public ExpressionSymbol this[int index]
    {
        get => Operands[index];
        set => Operands[index] = value;
    }

    public OperatorExpressionSymbol(params ExpressionSymbol[] operands)
    {
        Operands = operands;
    }

    public override string ToString()
    {
        var operands = string.Join(", ", Operands.Select(x => x.ToString()));

        return $"{Stringify(Operator)}({operands})";
    }
}

//*
//*
//* base classes, semantics oriented. 
//*
public abstract class UnaryExpressionSymbol : OperatorExpressionSymbol
{
    public StringSymbol Destination => (StringSymbol)Operands[0];
    public ExpressionSymbol Operand => Operands[1];

    protected UnaryExpressionSymbol(
        StringSymbol destination, 
        ExpressionSymbol operand
    )
    : base(destination, operand)
    {

    }
}

public abstract class BinaryExpressionSymbol : OperatorExpressionSymbol
{
    public StringSymbol Destination => (StringSymbol)Operands[0];
    public ExpressionSymbol LeftOperand => Operands[1];
    public ExpressionSymbol RightOperand => Operands[2];

    protected BinaryExpressionSymbol(
        StringSymbol destination, 
        ExpressionSymbol left, 
        ExpressionSymbol right
    )
    : base(destination, left, right )
    {

    }
}

public abstract class ArrayExpressionSymbol : OperatorExpressionSymbol
{
    public StringSymbol Destination => (StringSymbol)Operands[0];
    public IEnumerable<ExpressionSymbol> Expressions => GetValues();

    protected ArrayExpressionSymbol(StringSymbol destination, ExpressionSymbol[] expressions)
    : base(
        operands: new ExpressionSymbol[] { destination }
            .Concat(expressions)
            .ToArray()
    )
    {

    }

    private IEnumerable<ExpressionSymbol> GetValues()
    {
        for (int i = 1; i < OperandsCount; i++)
        {
            yield return Operands[i];
        }
    }
}

public abstract class PredicateExpressionSymbol : OperatorExpressionSymbol
{
    public StringSymbol Destination => (StringSymbol)Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public LambdaExpressionSymbol Lambda => (LambdaExpressionSymbol)Operands[2];

    protected PredicateExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {

    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({Source}, {Lambda})";
    }
}

//*
//*
//* arithmetic expression symbols.
//*
public class AddExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Add;

    public AddExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class SubtractExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Subtract;

    public SubtractExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class DivideExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Divide;

    public DivideExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class MultiplyExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Multiply;

    public MultiplyExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class ModuloExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Modulo;

    public ModuloExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

//*
//*
//* relational expression symbols.
//*

public class EqualsExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Equals;

    public EqualsExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class NotEqualsExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.NotEquals;

    public NotEqualsExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Less;

    public LessExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessEqualsExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.LessEquals;

    public LessEqualsExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Greater;

    public GreaterExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterEqualsExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.GreaterEquals;

    public GreaterEqualsExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

//*
//*
//* pattern match expression symbols.
//*

public class LikeExpressionSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Like;

    public LikeExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

//*
//*
//* logical expression symbols.
//*
public class AndExpressionSymbol : ArrayExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.And;

    public AndExpressionSymbol(
        StringSymbol destination, 
        ExpressionSymbol[] expressions
    ) 
    : base(destination, expressions)
    {
    }
}

public class OrExpressionSymbol : ArrayExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Or;

    public OrExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol[] expressions
    )
    : base(destination, expressions)
    {
    }
}

public class NotExpressionSymbol : UnaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Not;

    public NotExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol expression
    )
    : base(destination, expression)
    {
    }
}

//*
//*
//* semantic expression symbols.
//*
// [ string_literal, expression, ]
// "$anonymousType": [<destination>, { "id": "$item.id" }]
public class SelectExpressionSymbol : UnaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.AnonymousType;
    public AnonymousTypeExpressionSymbol TypeExpression => (AnonymousTypeExpressionSymbol)Operand;

    public SelectExpressionSymbol(
        StringSymbol destination,
        AnonymousTypeExpressionSymbol typeProjection
    )
    : base(destination, typeProjection)
    {

    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({TypeExpression})";
    }
}

//*
//*
//* query expression symbols.
//*

public class FilterExpressionSymbol : PredicateExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Filter;

    public FilterExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }
}

public class ProjectionExpressionSymbol : PredicateExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Select;

    public ProjectionExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }
}

//*
//*
//* aggregation expression symbols.
//*