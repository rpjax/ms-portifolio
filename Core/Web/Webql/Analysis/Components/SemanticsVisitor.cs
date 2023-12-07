using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Represents a visitor class for nodes in a syntax tree, used during the semantic analysis phase. <br/>
/// This class is designed to traverse the syntax tree, considering the semantic context, and potentially rewrite nodes for semantic purposes.
/// </summary>
public class SemanticsVisitor
{
    /// <summary>
    /// Visits a node in the syntax tree within a given semantic context.
    /// </summary>
    /// <param name="context">The semantic context in which the node is being visited.</param>
    /// <param name="node">The node to visit.</param>
    /// <returns>The visited node, potentially rewritten based on the semantic context.</returns>
    [return: NotNullIfNotNull("node")]
    public virtual Node? Visit(SemanticContext context, Node node)
    {
        switch (node.NodeType)
        {
            case NodeType.Literal:
                return Visit(context, node.As<LiteralNode>());

            case NodeType.Array:
                return Visit(context, node.As<ArrayNode>());

            case NodeType.LeftHandSide:
                return Visit(context, node.As<LhsNode>());

            case NodeType.RightHandSide:
                return Visit(context, node.As<RhsNode>());

            case NodeType.Expression:
                return Visit(context, node.As<ExpressionNode>());

            case NodeType.ScopeDefinition:
                return Visit(context, node.As<ObjectNode>());

            default:
                return node;
        }
    }

    /// <summary>
    /// Visits a ScopeDefinitionNode within a given semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the ScopeDefinitionNode.</param>
    /// <param name="node">The ScopeDefinitionNode to visit.</param>
    /// <returns>The visited ScopeDefinitionNode, possibly modified based on semantic rules.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(SemanticContext context, ObjectNode node)
    {
        for (int i = 0; i < node.Expressions.Length; i++)
        {
            node.Expressions[i] = Visit(context, node.Expressions[i]);
        }

        return node;
    }

    /// <summary>
    /// Visits an ArrayNode within a specific semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the ArrayNode.</param>
    /// <param name="node">The ArrayNode to visit.</param>
    /// <returns>The visited ArrayNode, potentially modified based on the context.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(SemanticContext context, ArrayNode node)
    {
        var baseStack = context.Stack;

        for (int i = 0; i < node.Values.Length; i++)
        {
            var stack = $"{baseStack}[{i}]";
            var subContext = new SemanticContext(context.Type, context, stack);

            node.Values[i] = Visit(subContext, node.Values[i]);
        }

        return node;
    }

    /// <summary>
    /// Visits an ExpressionNode within a given semantic context.
    /// </summary>
    /// <remarks>
    /// Modifies the ExpressionNode based on the semantic context, considering the type and properties of the context.
    /// </remarks>
    /// <param name="context">The semantic context for the ExpressionNode.</param>
    /// <param name="node">The ExpressionNode to visit.</param>
    /// <returns>The visited ExpressionNode, potentially modified based on semantic rules.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        var baseStack = context.Stack;
        var subStack = $".{node.Lhs.Value}";
        var stack = $"{baseStack}{subStack}";

        //*
        // { where: { foo: { } } }
        // $where.foo.$any[]
        //*
        if (!node.Lhs.IsOperator)
        {
            context = context.CreateSubContext(node.Lhs.Value, subStack);
        }
        else
        {
            var op = context.GetOperatorFromLhs(node.Lhs);
            var opIsIterator = HelperTools.OperatorIsIterator(op);
            var contextIsEnumerable = HelperTools.TypeIsEnumerable(context.Type);

            if (opIsIterator && contextIsEnumerable)
            {
                context = new(HelperTools.GetEnumerableType(context.Type), context, stack);
            }
            else
            {
                context = new(context.Type, context, stack);
            }
        }

        return new ExpressionNode(Visit(context, node.Lhs).As<LhsNode>(), Visit(context, node.Rhs).As<RhsNode>());
    }

    /// <summary>
    /// Visits a LhsNode within a semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the LhsNode.</param>
    /// <param name="node">The LhsNode to visit.</param>
    /// <returns>The visited LhsNode, unchanged as LhsNodes typically do not require semantic modifications.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(SemanticContext context, LhsNode node)
    {
        return node;
    }

    /// <summary>
    /// Visits an RhsNode within a given semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the RhsNode.</param>
    /// <param name="node">The RhsNode to visit.</param>
    /// <returns>The visited RhsNode, potentially modified based on the context.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(SemanticContext context, RhsNode node)
    {
        return new RhsNode(Visit(context, node.Value));
    }

    /// <summary>
    /// Visits a LiteralNode within a semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the LiteralNode.</param>
    /// <param name="node">The LiteralNode to visit.</param>
    /// <returns>The visited LiteralNode, unchanged as LiteralNodes typically do not require semantic modifications.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual Node? Visit(SemanticContext context, LiteralNode node)
    {
        return node;
    }

}
