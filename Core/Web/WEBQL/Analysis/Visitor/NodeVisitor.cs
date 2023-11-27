using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents a visitor for nodes in a syntax tree used during the semantic analysis phase. <br/>
/// This class is designed to traverse the syntax tree and potentially rewrite nodes for semantic purposes.
/// </summary>
public class NodeVisitor
{
    /// <summary>
    /// Visits a node in the syntax tree.
    /// </summary>
    /// <param name="node">The node to visit.</param>
    /// <returns>The visited node, potentially rewritten. Returns the original node if no rewriting occurs.</returns>
    [return: NotNullIfNotNull("node")]
    public virtual Node? Visit(Node node)
    {
        switch (node.NodeType)
        {
            case NodeType.Literal:
                return Visit(node.As<LiteralNode>());

            case NodeType.Array:
                return Visit(node.As<ArrayNode>());

            case NodeType.LeftHandSide:
                return Visit(node.As<LhsNode>());

            case NodeType.RightHandSide:
                return Visit(node.As<RhsNode>());

            case NodeType.Expression:
                return Visit(node.As<ExpressionNode>());

            case NodeType.ScopeDefinition:
                return Visit(node.As<ScopeDefinitionNode>());

            default:
                return node;
        }
    }

    /// <summary>
    /// Visits a ScopeDefinitionNode.
    /// </summary>
    /// <param name="node">The ScopeDefinitionNode to visit.</param>
    /// <returns>The visited ScopeDefinitionNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(ScopeDefinitionNode node)
    {
        for (int i = 0; i < node.Expressions.Length; i++)
        {
            node.Expressions[i] = Visit(node.Expressions[i]).As<ExpressionNode>();
        }

        return node;
    }

    /// <summary>
    /// Visits an ArrayNode.
    /// </summary>
    /// <param name="node">The ArrayNode to visit.</param>
    /// <returns>The visited ArrayNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(ArrayNode node)
    {
        for (int i = 0; i < node.Values.Length; i++)
        {
            node.Values[i] = Visit(node.Values[i]);
        }

        return node;
    }

    /// <summary>
    /// Visits an ExpressionNode.
    /// </summary>
    /// <param name="node">The ExpressionNode to visit.</param>
    /// <returns>The visited ExpressionNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(ExpressionNode node)
    {
        return new ExpressionNode(Visit(node.Lhs).As<LhsNode>(), Visit(node.Rhs).As<RhsNode>());
    }

    /// <summary>
    /// Visits an LhsNode.
    /// </summary>
    /// <param name="node">The LhsNode to visit.</param>
    /// <returns>The visited LhsNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(LhsNode node)
    {
        return node;
    }

    /// <summary>
    /// Visits an RhsNode.
    /// </summary>
    /// <param name="node">The RhsNode to visit.</param>
    /// <returns>The visited RhsNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(RhsNode node)
    {
        return new RhsNode(Visit(node.Value));
    }

    /// <summary>
    /// Visits a LiteralNode.
    /// </summary>
    /// <param name="node">The LiteralNode to visit.</param>
    /// <returns>The visited LiteralNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(LiteralNode node)
    {
        return node;
    }
}
