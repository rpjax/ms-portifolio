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
    SelectOld, // ok
    AnonymousType,
    MemberAccess,

    // Collection Manipulation Operators
    Filter, // ok
    Select, // ok
    Transform, // ok
    SelectMany,
    Limit, // ok
    Skip, // ok

    //*
    // Queryable Ordering Operator *TODO...
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

//*
//*
//*
// NOTE: Although these enums are primarily used in semantic analysis, they're placed here to streamline
// the management of operator modifications. Grouping them together simplifies making changes to operator
// definitions—whether it's adding new operators, altering existing ones, or removing them. This approach 
// ensures that semantic analysis remains consistent with the operators' definitions, facilitating the 
// identification and correction of any analysis mismatches or missing cases.
//*

public enum OperatorParametrizationType
{
    Specific,
    Unary,
    Binary,
    Predicate,
    Array
}

public enum OperatorSemanticType
{
    Arithmetic,
    Relational,
    Logical,
    Semantic,
    CollectionManipulation,
    CollectionAggregation
}

public enum ArithmeticOperatorTypes
{
    Add,
    Subtract,
    Divide,
    Multiply,
    Modulo
}

public enum RelationalOperatorTypes
{
    Equals,
    NotEquals,
    Less,
    LessEquals,
    Greater,
    GreaterEquals,
    Like,
    RegexMatch
}

public enum LogicalOperatorTypes
{
    Or,
    And,
    Not
}

public enum SemanticOperatorType
{
    Expr,
    Parse,
    SelectOld,
    AnonymousType,
    MemberAccess
}

public enum CollectionManipulationOperatorType
{
    Filter,
    Select,
    Transform,
    SelectMany,
    Limit,
    Skip
}

public enum CollectionAggregationOperatorTypes
{
    Count,
    Index,
    Any,
    All,
    Min,
    Max,
    Sum,
    Average
}
