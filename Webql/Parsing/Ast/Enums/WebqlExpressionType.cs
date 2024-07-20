
namespace Webql.Parsing.Ast;

/// <summary>
/// Represents the type of a WebQL expression.
/// </summary>
public enum WebqlExpressionType
{
    /// <summary>
    /// Represents a literal expression.
    /// </summary>
    Literal,

    /// <summary>
    /// Represents a temporary declaration expression.
    /// </summary>
    TemporaryDeclaration,

    /// <summary>
    /// Represents a reference expression.
    /// </summary>
    Reference,

    /// <summary>
    /// Represents a member access expression.
    /// </summary>
    MemberAccess,

    /// <summary>
    /// Represents an operation expression.
    /// </summary>
    Operation,

    /// <summary>
    /// Represents a type conversion expression.
    /// </summary>
    TypeConversion,

    /// <summary>
    /// Represents an anonymous object expression.
    /// </summary>
    AnonymousObject,
}
