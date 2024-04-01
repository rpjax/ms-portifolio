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

public class DivideExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Divide;

    public DivideExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class MultiplyExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Multiply;

    public MultiplyExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class ModuloExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Modulo;

    public ModuloExprSymbol(
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

public class EqualsExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Equals;

    public EqualsExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class NotEqualsExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.NotEquals;

    public NotEqualsExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Less;

    public LessExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessEqualsExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.LessEquals;

    public LessEqualsExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Greater;

    public GreaterExprSymbol(
        StringSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterEqualsExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.GreaterEquals;

    public GreaterEqualsExprSymbol(
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

public class LikeExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Like;

    public LikeExprSymbol(
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
// "$type": [<destination>, { "id": "$item.id" }]
public class TypeExpressionSymbol : UnaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Type;
    public TypeProjectionExpressionSymbol TypeExpression => (TypeProjectionExpressionSymbol)Operand;

    public TypeExpressionSymbol(
        StringSymbol destination,
        TypeProjectionExpressionSymbol typeProjection
    )
    : base(destination, typeProjection)
    {

    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({TypeExpression})";
    }
}

// [ string_literal, expression, ]
// "$memberAccess": [<destination>, <member>, <expression>]
public class MemberAccessExprSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.MemberAccess;
    public StringSymbol MemberName => (StringSymbol)Operands[0];
    public ExpressionSymbol Operand => Operands[1];

    public MemberAccessExprSymbol(
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
    public override OperatorType Operator { get; } = OperatorType.Project;

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