using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        var baseStack = context.Label;

        for (int i = 0; i < node.Values.Length; i++)
        {
            var stack = $"{baseStack}[{i}]";
            var subContext = new SemanticContext(context.Type, context, stack);

            node.Values[i] = Visit(subContext, node.Values[i]);
        }

        return node;
    }

    /// <summary>
    /// Visits an ExpressionNode within a specific semantic context.
    /// </summary>
    /// <remarks>
    /// This method adapts the ExpressionNode based on the provided semantic context. <br/>
    /// It evaluates and modifies the node in consideration of the context's type and properties, <br/>
    /// thus aligning it with the applicable semantic rules.
    /// </remarks>
    /// <param name="context">The semantic context in which to evaluate the ExpressionNode.</param>
    /// <param name="node">The ExpressionNode to be visited and potentially transformed.</param>
    /// <returns>The modified ExpressionNode after applying semantic context-based rules.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        var baseStack = context.Label;
        var subStack = $".{node.Lhs.Value}";
        var stack = $"{baseStack}{subStack}";

        if (node.Lhs.IsReference && !context.EnableNavigation)
        {
            return new ExpressionNode(Visit(context, node.Lhs), Visit(context, node.Rhs));
        }

        if (node.Lhs.IsReference)
        {
            context = context.GetReferenceContext(node.Lhs.Value, subStack);
        }

        if (node.Lhs.IsOperator)
        {
            var op = HelperTools.ParseOperatorString(node.Lhs.Value);

            if(op == null)
            {
                //*
                // The decision to not throw an exception here is intentional and serves a specific purpose. 
                // The "$Expr" operator, which comes into play in this scenario, is designed to create a new expression scope without altering the semantic context. It effectively isolates expressions or contexts, allowing for independent evaluation and interpretation within the established semantic rules. 
                // This approach avoids introducing implicit behavior or validation that might be unintended in derived classes of SemanticVisitor.
                // In essence, the "$Expr" operator serves as a means to compartmentalize and isolate parts of the syntax tree, ensuring that each segment is processed in its own right, without unintended interference or assumptions.
                // This is crucial for maintaining the integrity and modularity of the semantic analysis process.
                //*
                op = Operator.Expr;
            }

            var opType = HelperTools.GetOperatorType(op.Value);
            var operatorIsQueryable = opType == OperatorType.Queryable;
            var contextIsQueryable = context.IsQueryable();

            if (op == Operator.Project)
            {
                //*
                // Disables specific analysis features when entering a projection context via "$project" operator. 
                // This adjustment is crucial as the query semantics shift in a projection context. 
                // Without this change to projection semantics, analysis could incorrectly handle 
                // member access expressions and implicit syntax, leading to potential failures.
                //*
                context.SetToProjectionSematics();
            }

            if (operatorIsQueryable && contextIsQueryable)
            {
                context = new SemanticContext(context.GetQueryableElementType(), context, stack);
            }
            else
            {
                context = new SemanticContext(context.Type, context, stack);
            }
        }

        return new ExpressionNode(Visit(context, node.Lhs), Visit(context, node.Rhs));
    }

    /// <summary>
    /// Visits a LhsNode within a semantic context.
    /// </summary>
    /// <param name="context">The semantic context for the LhsNode.</param>
    /// <param name="node">The LhsNode to visit.</param>
    /// <returns>The visited LhsNode, unchanged as LhsNodes typically do not require semantic modifications.</returns>
    [return: NotNullIfNotNull("node")]
    protected virtual LhsNode? Visit(SemanticContext context, LhsNode node)
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
    protected virtual RhsNode? Visit(SemanticContext context, RhsNode node)
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

    //*
    // Helper Methods Section
    //*

    /// <summary>
    /// Converts a string representation of an operator into its corresponding <see cref="Operator"/> enum value.
    /// </summary>
    /// <param name="context">The semantic context for the operator parsing.</param>
    /// <param name="value">The string representation of the operator.</param>
    /// <returns>The Operator enum value.</returns>
    /// <exception cref="SemanticException">Thrown when the operator string is not recognized.</exception>
    protected Operator ParseOperatorString(SemanticContext context, string value)
    {
        var op = HelperTools.ParseOperatorString(value);

        if (op == null)
        {
            throw SemanticThrowHelper.UnknownOrUnsupportedOperator(context, value);
        }

        return op.Value;
    }

}
