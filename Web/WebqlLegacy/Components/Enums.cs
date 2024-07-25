namespace ModularSystem.Webql;

public enum NodeType
{
    Literal,
    Null,
    Array,
    LeftHandSide,
    RightHandSide,
    Expression,
    Object,
}

/// <summary>
/// Reflects the syntax of a JSON document.
/// </summary>
public enum RhsType
{
    Literal,
    Object,
    Array
}

public enum OperatorOld
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

/// <summary>
/// All operators that accept and return a queryable.
/// </summary>
public enum OperatorTypeOld
{
    Arithmetic,
    Relational,
    PatternRelational,
    Logical,
    Semantic,
    Queryable,
    Aggregation
}

public enum OperatorCategory
{
    Unary,
    Binary,
    Ternary
}
