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
    And
}

public enum ArrayOperator
{
    Index,
}

public enum OrderDirection
{
    Ascending,
    Descending
}