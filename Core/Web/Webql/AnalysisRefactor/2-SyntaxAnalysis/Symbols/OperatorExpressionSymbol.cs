namespace ModularSystem.Webql.Analysis.Symbols;

public interface IOperatorExpressionSymbol : IExpressionSymbol
{
    OperatorType Operator { get; }
    ExpressionSymbol[] Operands { get; }
}

public abstract class OperatorExpressionSymbol : ExpressionSymbol, IOperatorExpressionSymbol
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
public abstract class UnaryOperatorExpressionSymbol : OperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Operand => Operands[1];

    protected UnaryOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol operand
    )
    : base(destination, operand)
    {
  
    }
}

public abstract class BinaryOperatorExpressionSymbol : OperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol LeftOperand => Operands[1];
    public ExpressionSymbol RightOperand => Operands[2];

    protected BinaryOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol left, 
        ExpressionSymbol right
    )
    : base(destination, left, right )
    {

    }
}

public abstract class ArrayOperatorExpressionSymbol : OperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public IEnumerable<ExpressionSymbol> Expressions => GetValues();

    protected ArrayOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol[] expressions
    )
    : base(new ExpressionSymbol[] { destination }.Concat(expressions).ToArray())
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

public abstract class PredicateOperatorExpressionSymbol : OperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public LambdaExpressionSymbol Lambda => (LambdaExpressionSymbol)Operands[2];

    protected PredicateOperatorExpressionSymbol(
        ExpressionSymbol destination,
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
public interface IArithmeticOperatorExpressionSymbol
{

}

public abstract class ArithmeticOperatorExpressionSymbol : BinaryOperatorExpressionSymbol, IArithmeticOperatorExpressionSymbol
{
    protected ArithmeticOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class AddOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Add;

    public AddOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class SubtractOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Subtract;

    public SubtractOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class DivideOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Divide;

    public DivideOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class MultiplyOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Multiply;

    public MultiplyOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class ModuloOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Modulo;

    public ModuloOperatorExpressionSymbol(
        ExpressionSymbol destination,
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
public interface IRelationalOperatorExpressionSymbol 
{

}

public abstract class RelationalOperatorExpressionSymbol : BinaryOperatorExpressionSymbol, IRelationalOperatorExpressionSymbol
{
    protected RelationalOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class EqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Equals;

    public EqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class NotEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.NotEquals;

    public NotEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Less;

    public LessOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LessEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.LessEquals;

    public LessEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Greater;

    public GreaterOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class GreaterEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.GreaterEquals;

    public GreaterEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
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
public interface IStringRelationalOperatorExpressionSymbol
{

}

public abstract class StringRelationalOperatorExpressionSymbol 
    : BinaryOperatorExpressionSymbol, IStringRelationalOperatorExpressionSymbol
{
    protected StringRelationalOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LikeOperatorExpressionSymbol : StringRelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Like;

    public LikeOperatorExpressionSymbol(
        ExpressionSymbol destination,
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
public interface ILogicalOperatorExpressionSymbol : IOperatorExpressionSymbol
{

}

public class AndOperatorExpressionSymbol : ArrayOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.And;

    public AndOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol[] expressions
    ) 
    : base(destination, expressions)
    {
    }
}

public class OrOperatorExpressionSymbol : ArrayOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Or;

    public OrOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol[] expressions
    )
    : base(destination, expressions)
    {
    }
}

public class NotOperatorExpressionSymbol : UnaryOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Not;

    public NotOperatorExpressionSymbol(
        ExpressionSymbol destination,
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

//*
//*
//* collection manipulation expression symbols.
//*
public interface ICollectionManipulationOperatorExpression : IOperatorExpressionSymbol
{

}

public class FilterOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Filter;

    public FilterOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }
}

// [ string_literal, expression, ]
// "$anonymousType": [<destination>, { "id": "$item.id" }]
public class SelectOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.AnonymousType;
    public ExpressionSymbol Destination => Operands[0];
    public AnonymousTypeExpressionSymbol TypeExpression => (AnonymousTypeExpressionSymbol)Operands[1];

    public SelectOperatorExpressionSymbol(
        ExpressionSymbol destination,
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

public class ProjectionOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Select;

    public ProjectionOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }
}

public class LimitOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Limit;
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public ExpressionSymbol Value => Operands[2];

    public LimitOperatorExpressionSymbol(
        StringSymbol destination, 
        ExpressionSymbol source, 
        ExpressionSymbol value
    )         
    : base(destination, source, value)
    {
    }
}

public class SkipOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Skip;
    public StringSymbol Destination => (StringSymbol)Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public ExpressionSymbol Value => Operands[2];

    public SkipOperatorExpressionSymbol(
        StringSymbol destination,
        ExpressionSymbol source,
        ExpressionSymbol value
    )
    : base(destination, source, value)
    {
    }
}

//*
//*
//* collection aggregation expression symbols.
//*
public interface ICollectionAggregationOperatorExpression : IOperatorExpressionSymbol
{

}

