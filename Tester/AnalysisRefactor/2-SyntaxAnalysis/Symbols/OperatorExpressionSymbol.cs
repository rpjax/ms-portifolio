namespace ModularSystem.Webql.Analysis.Symbols;

public abstract class OperatorExpressionSymbol : ExpressionSymbol
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.Operator;
    public abstract OperatorType Operator { get; }
    public ExpressionSymbol[] Operands { get; init; }
}

public class OpExpressionSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; }

    public OpExpressionSymbol(OperatorType @operator, ExpressionSymbol[] operands)
    {
        Operator = @operator;
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
//* base classes
//*

public abstract class UnaryExpressionSymbol : OperatorExpressionSymbol
{
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol Operand { get; }

    protected UnaryExpressionSymbol(DestinationSymbol destination, ExpressionSymbol operand)
    {
        Destination = destination;
        Operand = operand;
    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({Operand})";
    }
}

public abstract class BinaryExpressionSymbol : OperatorExpressionSymbol 
{
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol LeftOperand { get; }
    public ExpressionSymbol RightOperand { get; }

    protected BinaryExpressionSymbol(DestinationSymbol destination, ExpressionSymbol left, ExpressionSymbol right)
    {
        Destination = destination;
        LeftOperand = left;
        RightOperand = right;
    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({Destination}, {LeftOperand}, {RightOperand})";
    }
}

public abstract class QueryExpressionSymbol : OperatorExpressionSymbol
{
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol Source { get; }
    public LambdaExpressionSymbol Lambda { get; }

    protected QueryExpressionSymbol(DestinationSymbol destination, ExpressionSymbol source, LambdaExpressionSymbol lambda)
    {
        Destination = destination;
        Source = source;
        Lambda = lambda;
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

public class AddExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Add;

    public AddExprSymbol(
        DestinationSymbol destination, 
        ExpressionSymbol left,
        ExpressionSymbol right
    ) 
    : base(destination, left, right)
    {
    }
}

public class SubtractExprSymbol : BinaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Subtract;

    public SubtractExprSymbol(
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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
        DestinationSymbol destination,
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

public class AndExprSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.And;
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol[] Expressions { get; }

    public AndExprSymbol(DestinationSymbol destination, ExpressionSymbol[] expressions)
    {
        Destination = destination;
        Expressions = expressions;
    }

    public override string ToString()
    {
        var argsStr = string.Join(" ", Expressions.Select(x => $"{x};"));

        return $"{Stringify(Operator)}({Destination}, [{argsStr}])";
    }
}

public class OrExprSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Or;
    public DestinationSymbol Destination { get; }
    public ExpressionSymbol[] Expressions { get; }

    public OrExprSymbol(DestinationSymbol destination, ExpressionSymbol[] expressions)
    {
        Destination = destination;
        Expressions = expressions;
    }

    public override string ToString()
    {
        var argsStr = string.Join(" ", Expressions.Select(x => $"{x};"));

        return $"{Stringify(Operator)}({Destination}, [{argsStr}])";
    }
}

public class NotExprSymbol : UnaryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Not;

    public NotExprSymbol(
        DestinationSymbol destination, 
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

public class TypeExprSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Type;
    public ProjectionObjectSymbol ProjectionObject { get; }

    public TypeExprSymbol(ProjectionObjectSymbol projectionObject)
    {
        ProjectionObject = projectionObject;
    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({ProjectionObject})";
    }
}

public class MemberAccessExprSymbol : OperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.MemberAccess;
    public ExpressionSymbol Operand { get; }
    public string MemberName { get; }

    public MemberAccessExprSymbol(ExpressionSymbol operand, string memberName)
    {
        Operand = operand;
        MemberName = memberName;
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

public class FilterExprSymbol : QueryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Filter;

    public FilterExprSymbol(
        DestinationSymbol destination, 
        ExpressionSymbol source, 
        LambdaExpressionSymbol lambda
    ) 
    : base(destination, source, lambda)
    {
    }
}

public class ProjectionExprSymbol : QueryExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Project;

    public ProjectionExprSymbol(
        DestinationSymbol destination,
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

