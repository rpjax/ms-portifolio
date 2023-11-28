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
                return VisitLiteral(node.As<LiteralNode>());

            case NodeType.Array:
                return VisitArray(node.As<ArrayNode>());

            case NodeType.LeftHandSide:
                return VisitLhs(node.As<LhsNode>());

            case NodeType.RightHandSide:
                return VisitRhs(node.As<RhsNode>());

            case NodeType.Expression:
                return VisitExpression(node.As<ExpressionNode>());

            case NodeType.ScopeDefinition:
                return VisitScope(node.As<ScopeDefinitionNode>());

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
    protected virtual Node? VisitScope(ScopeDefinitionNode node)
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
    protected virtual Node? VisitArray(ArrayNode node)
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
    protected virtual Node? VisitExpression(ExpressionNode node)
    {
        return new ExpressionNode(Visit(node.Lhs).As<LhsNode>(), Visit(node.Rhs).As<RhsNode>());
    }

    /// <summary>
    /// Visits an LhsNode.
    /// </summary>
    /// <param name="node">The LhsNode to visit.</param>
    /// <returns>The visited LhsNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? VisitLhs(LhsNode node)
    {
        return node;
    }

    /// <summary>
    /// Visits an RhsNode.
    /// </summary>
    /// <param name="node">The RhsNode to visit.</param>
    /// <returns>The visited RhsNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? VisitRhs(RhsNode node)
    {
        return new RhsNode(Visit(node.Value));
    }

    /// <summary>
    /// Visits a LiteralNode.
    /// </summary>
    /// <param name="node">The LiteralNode to visit.</param>
    /// <returns>The visited LiteralNode. Subclasses can override this method to provide custom handling.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? VisitLiteral(LiteralNode node)
    {
        return node;
    }
}
