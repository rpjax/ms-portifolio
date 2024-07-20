namespace Webql.Parsing.Ast;

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
    Aggregate,
    New, // todo
    //Parse, // ok
    //Type,

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
    Contains,
    Index,
    Any, // ok
    All, // ok
    Min, // ok
    Max, // ok
    Sum,
    Average,
}

/// <summary>
/// Represents the category of an operator.
/// </summary>
public enum WebqlOperatorCategory
{
    Arithmetic,
    Relational,
    StringRelational,
    Logical,
    Semantic,
    CollectionManipulation,
    CollectionAggregation,
}

/// <summary>
/// Represents the number of arguments or operands taken by an operator.
/// </summary>
public enum WebqlOperatorArity
{
    //Nullary,
    Unary,
    Binary,
    //Ternary
}

/*
 * Helper Enumerations
 */

public enum WebqlUnaryOperator
{
    Not,
    Count,
    Aggregate,
    New
}

public enum WebqlBinaryOperator
{
    Add, 
    Subtract, 
    Divide, 
    Multiply,
    Modulo, 
    Equals, 
    NotEquals,
    Less, 
    LessEquals, 
    Greater, 
    GreaterEquals, 
    Like, 
    RegexMatch,
    Or, 
    And, 
    Filter, 
    Select, 
    SelectMany,
    Limit, 
    Skip, 
    Contains,
    Index,
    Any, 
    All, 
    Min, 
    Max, 
    Sum,
    Average,
}

public enum WebqlSemanticOperator
{
    Aggregate,
    New
}

public enum WebqlCollectionOperator
{
    Filter,
    Select,
    SelectMany,
    Limit,
    Skip,
    Count,
    Contains,
    Index,
    Any,
    All,
    Min,
    Max,
    Sum,
    Average,
}

public enum WebqlCollectionManipulationOperator
{
    Filter,
    Select,
    SelectMany,
    Limit,
    Skip,
}

public enum WebqlCollectionAggregationOperator
{
    Count,
    Contains,
    Index,
    Any,
    All,
    Min,
    Max,
    Sum,
    Average,
}
