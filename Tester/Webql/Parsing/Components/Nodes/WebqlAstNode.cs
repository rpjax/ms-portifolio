namespace Webql.DocumentSyntax.Parsing.Components;

/// <summary>
/// Represents a node in the abstract syntax tree for the WebQL language.
/// </summary>
public abstract class WebqlAstNode
{
    public abstract WebqlNodeType NodeType { get; }
}
