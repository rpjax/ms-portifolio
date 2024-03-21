namespace ModularSystem.Webql.Analysis.Symbols;

public enum ExprOp
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
    Literal, // ok
    Select, // ok

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

public abstract class ExprSymbol : Symbol
{
    
}

//*
//*
//* base classes
//*

public abstract class BinaryExprSymbol : ExprSymbol 
{
    public BinaryArgumentsSymbol Arguments { get; }

    protected BinaryExprSymbol(BinaryArgumentsSymbol arguments)
    {
        Arguments = arguments;
    }
}

//*
//*
//* arithmetic expression symbols.
//*

public class AddExprSymbol : BinaryExprSymbol
{
    public AddExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"${StringifyExprOp(ExprOp.Add)}{Arguments}";
    }
}

public class SubtractExprSymbol : BinaryExprSymbol
{
    public SubtractExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Subtract)}{Arguments}";
    }
}

public class DivideExprSymbol : BinaryExprSymbol
{
    public DivideExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Divide)}{Arguments}";
    }
}

public class MultiplyExprSymbol : BinaryExprSymbol
{
    public MultiplyExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Multiply)}{Arguments}";
    }
}

public class ModuloExprSymbol : BinaryExprSymbol
{
    public ModuloExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Modulo)}{Arguments}";
    }
}

//*
//*
//* relational expression symbols.
//*

public class EqualsExprSymbol : BinaryExprSymbol
{
    public EqualsExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Equals)}{Arguments}";
    }
}

public class NotEqualsExprSymbol : BinaryExprSymbol
{
    public NotEqualsExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.NotEquals)}{Arguments}";
    }
}

public class LessExprSymbol : BinaryExprSymbol
{
    public LessExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Less)}{Arguments}";
    }
}

public class LessEqualsExprSymbol : BinaryExprSymbol
{
    public LessEqualsExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.LessEquals)}{Arguments}";
    }
}

public class GreaterExprSymbol : BinaryExprSymbol
{
    public GreaterExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Greater)}{Arguments}";
    }
}

public class GreaterEqualsExprSymbol : BinaryExprSymbol
{
    public GreaterEqualsExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.GreaterEquals)}{Arguments}";
    }
}

//*
//*
//* pattern match expression symbols.
//*

public class LikeExprSymbol : BinaryExprSymbol
{
    public LikeExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Like)}{Arguments}";
    }
}

//*
//*
//* logical expression symbols.
//*

public class AndExprSymbol : ExprSymbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol[] Arguments { get; }

    public AndExprSymbol(DestinationSymbol destination, ArgumentSymbol[] arguments)
    {
        Destination = destination;
        Arguments = arguments;
    }

    public override string ToString()
    {
        var argsStr = string.Join(" ", Arguments.Select(x => $"{x};"));

        return $"{StringifyExprOp(ExprOp.And)}({Destination}, [{argsStr}])";
    }
}

public class OrExprSymbol : ExprSymbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol[] Arguments { get; }

    public OrExprSymbol(DestinationSymbol destination, ArgumentSymbol[] arguments)
    {
        Destination = destination;
        Arguments = arguments;
    }

    public override string ToString()
    {
        var argsStr = string.Join(" ", Arguments.Select(x => $"{x};"));

        return $"{StringifyExprOp(ExprOp.Or)}({Destination}, [{argsStr}])";
    }
}

public class NotExprSymbol : ExprSymbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol Argument { get; }

    public NotExprSymbol(DestinationSymbol destination, ArgumentSymbol argument)
    {
        Destination = destination;
        Argument = argument;
    }

    public override string ToString()
    {
        return $"{StringifyExprOp(ExprOp.Not)}({Destination}, [{Argument}])";
    }
}

//*
//*
//* semantic expression symbols.
//*

//*
//*
//* query expression symbols.
//*

public class FilterExprSymbol : ExprSymbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol Source { get; }
    public LambdaSymbol Lambda { get; }

    public FilterExprSymbol(DestinationSymbol destination, ArgumentSymbol source, LambdaSymbol lambda)
    {
        Destination = destination;
        Source = source;
        Lambda = lambda;
    }

    public override string ToString()
    {
        return $"${ExprOp.Filter.ToString().ToLower()} ({Destination}, {Source}, {Lambda})";
    }
}

public class ProjectionExprSymbol : ExprSymbol
{
    public DestinationSymbol Destination { get; }
    public ArgumentSymbol Source { get; }
    public LambdaSymbol Lambda { get; }

    public ProjectionExprSymbol(DestinationSymbol destination, ArgumentSymbol source, LambdaSymbol lambda)
    {
        Destination = destination;
        Source = source;
        Lambda = lambda;
    }

    public override string ToString()
    {
        return $"${ExprOp.Filter.ToString().ToLower()} ({Destination}, {Source}, {Lambda})";
    }
}

//*
//*
//* aggregation expression symbols.
//*

