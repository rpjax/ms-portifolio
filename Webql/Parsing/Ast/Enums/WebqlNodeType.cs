namespace Webql.Parsing.Ast;

/// <summary>
/// Represents the type of a Webql node.
/// </summary>
public enum WebqlNodeType
{
    /// <summary>
    /// Represents a query node.
    /// </summary>
    Query,

    /// <summary>
    /// Represents an expression node.
    /// </summary>
    Expression,

    /// <summary>
    /// Represents a property of an anonymous object node.
    /// </summary>
    AnonymousObjectProperty,
}
