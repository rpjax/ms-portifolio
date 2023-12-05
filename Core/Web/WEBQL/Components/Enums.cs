namespace ModularSystem.Webql;

public enum NodeType
{
    Literal,
    Array,
    LeftHandSide,
    RightHandSide,
    Expression,
    ScopeDefinition
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

public enum Operator
{
    Invalid,
    Equals,
    Not,
    Less,
    LesserEquals,
    Greater,
    GreaterEquals,
    Like,
    Any,
    All
}

public enum OperatorV2
{
    // Arithmetic operators
    Add,
    Subtract,
    Divide,
    Multiply,
    Modulo,

    // Relational Operators
    Equals,
    NotEquals,
    Less,
    LessEquals,
    Greater,
    GreaterEquals,

    // Logical Operators
    Or,
    And,
    Not,

    // Queryable Operators
    Filter,
    Project,
    Limit,
    Skip,
    Size,
    Index,
    Any,
    All,

    // Semantic Operators
    Expr,
    Literal,
    Select,

    // Aggregation Operators
    Count,
    Min,
    Max,
    Sum,
    Average
}

public enum OperatorType
{
    Arithmetic,
    Relational,
    Logical,
    Array
}

public enum ArithmeticOperator
{
    Add,
    Subtract,
    Divide,
    Multiply,
    Modulo
}

public enum RelationalOperator
{
    Equals,
    NotEquals,
    Less,
    LessEquals,
    Greater,
    GreaterEquals,
}

public enum LogicalOperator
{
    Or,
    And,
    Not
}

public enum QueryableOperator
{
    Filter,
    Project,
    Limit,
    Skip,
    Size,
    Index,
    Any,
    All
}

public enum OrderDirection
{
    Ascending,
    Descending
}