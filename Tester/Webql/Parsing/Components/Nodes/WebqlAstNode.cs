namespace Webql.DocumentSyntax.Parsing.Components;

/// <summary>
/// Represents a node in the abstract syntax tree for the WebQL language.
/// </summary>
public abstract class WebqlAstNode
{
    /// <summary>
    /// Gets the type of the node. It is used to determine the kind of node before casting.
    /// </summary>
    public abstract WebqlNodeType NodeType { get; }

    /// <summary>
    /// Gets the syntax metadata associated with the node.
    /// </summary>
    public abstract SyntaxNodeMetadata Metadata { get; }
}
