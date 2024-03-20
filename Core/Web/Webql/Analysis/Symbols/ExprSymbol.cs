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

public abstract class BinaryExprSymbol : ExprSymbol 
{
    public BinaryArgumentsSymbol Arguments { get; }

    protected BinaryExprSymbol(BinaryArgumentsSymbol arguments)
    {
        Arguments = arguments;
    }
}

public class AddExprSymbol : BinaryExprSymbol
{
    public AddExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"${ExprOp.Add.ToString().ToLower()} {Arguments}";
    }
}

public class SubtractExprSymbol : BinaryExprSymbol
{
    public SubtractExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{ExprOp.Subtract} {Arguments}";
    }
}

public class LikeExprSymbol : BinaryExprSymbol
{
    public LikeExprSymbol(BinaryArgumentsSymbol arguments) : base(arguments)
    {
    }

    public override string ToString()
    {
        return $"{ExprOp.Like} {Arguments}";
    }
}

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
