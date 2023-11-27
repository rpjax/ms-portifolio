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
    Invalid,
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

public enum BinaryOperator
{
    Or,
    And
}

public enum OrderDirection
{
    Ascending,
    Descending
}