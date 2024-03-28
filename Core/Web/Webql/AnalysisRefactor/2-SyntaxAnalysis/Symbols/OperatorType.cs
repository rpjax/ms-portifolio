namespace ModularSystem.Webql.Analysis.Symbols;

public enum OperatorType
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
    MemberAccess,

    // Queryable Operators
    Filter, // ok
    Project, // ok
    Transform, // ok
    SelectMany,
    Limit, // ok
    Skip, // ok

    //*
    // Queryable Ordering Operator *TODO...
    // OrderAsc,
    // OrderDesc,
    //*

    // Aggregation Operators
    Count, // ok
    Index,
    Any, // ok
    All, // ok
    Min, // ok
    Max, // ok
    Sum,
    Average,
}

