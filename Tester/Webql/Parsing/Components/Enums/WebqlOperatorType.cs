namespace Webql.DocumentSyntax.Parsing.Components;

public enum WebqlOperatorType
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

    // String Relational Operators
    Like, // ok
    RegexMatch,

    // Logical Operators
    Or, // ok
    And, // ok
    Not, // ok

    // Semantic Operators
    //Expr, // ok
    //Parse, // ok
    //Type,
    //New, // todo
    //MemberAccess,

    // Collection Manipulation Operators
    // TODO: review those operators, i want to reformulate projection.
    Filter, // ok
    Select, // ok
    SelectMany,
    Limit, // ok
    Skip, // ok

    //*
    // TODO...
    // OrderAsc,
    // OrderDesc,
    //*

    // Collection Aggregation Operators
    Count, // ok
    Index,
    Any, // ok
    All, // ok
    Min, // ok
    Max, // ok
    Sum,
    Average,
}

