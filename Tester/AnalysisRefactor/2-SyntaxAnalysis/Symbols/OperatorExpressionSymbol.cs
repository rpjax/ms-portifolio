namespace ModularSystem.Webql.Analysis.Symbols;

public enum ExpressionOperator
{
    // Arithmetic operators
    Add, // ok
    Subtract, // ok
    Divide, // ok
    Multiply, // ok
    Modulo, // ok

    // Relational Operators
    Equals, // ok
    NotEquals, // ok
    Less, // ok
    LessEquals, // ok
    Greater, // ok
    GreaterEquals, // ok

    // Pattern Relational Operators
    Like, // ok
    RegexMatch,

    // Logical Operators
    Or, // ok
    And, // ok
    Not, // ok

    // Semantic Operators
    Expr, // ok
    Parse, // ok
    Select, // ok
    Type,

    // Queryable Operators
    Filter, // ok
    Project, // ok
    Transform, // ok
    SelectMany,
    Limit, // ok
    Skip, // ok
    Count, // ok
    Index,
    Any, // ok
    All, // ok
    // Queryable Ordering Operator *TODO...
    //OrderAsc,
    //OrderDesc,

    // Aggregation Operators
    Min, // ok
    Max, // ok
    Sum,
    Average,
}

public abstract class OperatorExpressionSymbol : ExpressionSymbol
{
    public abstract ExpressionOperator Operator { get; }
    public override ExpressionType ExpressionType { get; } = ExpressionType.Operator;
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
    public LambdaSymbol Lambda { get; }

    protected QueryExpressionSymbol(DestinationSymbol destination, ExpressionSymbol source, LambdaSymbol lambda)
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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Add;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Subtract;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Divide;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Multiply;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Modulo;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Equals;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.NotEquals;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Less;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.LessEquals;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Greater;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.GreaterEquals;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Like;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.And;
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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Or;
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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Not;

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
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Type;
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

//*
//*
//* query expression symbols.
//*

public class FilterExprSymbol : QueryExpressionSymbol
{
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Filter;

    public FilterExprSymbol(
        DestinationSymbol destination, 
        ExpressionSymbol source, 
        LambdaSymbol lambda
    ) 
    : base(destination, source, lambda)
    {
    }
}

public class ProjectionExprSymbol : QueryExpressionSymbol
{
    public override ExpressionOperator Operator { get; } = ExpressionOperator.Project;

    public ProjectionExprSymbol(
        DestinationSymbol destination,
        ExpressionSymbol source,
        LambdaSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }
}

//*
//*
//* aggregation expression symbols.
//*

