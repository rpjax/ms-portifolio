using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// A visitor class for applying filter semantics by potentially rewriting parts of the syntax tree. <br/>
/// This class extends NodeVisitor and provides specialized visit methods to handle different node types.
/// </summary>
public class FilterSemanticsRewriterVisitor : SemanticsVisitor
{
    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        var rhsIsArray = node.Rhs.Value.NodeType == NodeType.Array;
        var rhsIsScope = node.Rhs.Value.NodeType == NodeType.ScopeDefinition;

        if (node.Lhs.IsOperator)
        {
            var op = context.GetOperatorFromLhs(node.Lhs);
            var opIsIterator = HelperTools.OperatorIsIterator(op);

            if (rhsIsArray && !opIsIterator)
            {
                return RewriteOperatorExpressionWithArrayRhs(context, node);
            }
            if (rhsIsScope && opIsIterator)
            {
                return RewriteIteratorExpressionWithScopeRhs(context, node);
            }
        }
        else
        {
            if (node.Rhs.Value.NodeType == NodeType.Literal)
            {
                return RewriteMemberExpressionWithLiteralRhs(context, node);
            }
            if (node.Rhs.Value.NodeType == NodeType.Array)
            {
                return RewriteMemberExpressionWithArrayRhs(context, node);
            }
        }

        return base.Visit(context, node);
    }

    /// <summary>
    /// Rewrites an operator expression with an array on the right-hand side.
    /// </summary>
    /// <remarks>
    /// Transformation example: <br/>
    /// Before: { "$op": ["value1", "value2"] } <br/>
    /// After:  { "$any": [{ "$op": "value1" }, { "$op": "value2" }] } 
    /// </remarks>
    /// <param name="node">The ExpressionNode to rewrite.</param>
    /// <returns>The rewritten node.</returns>
    [return: NotNullIfNotNull("node")]
    private ExpressionNode? RewriteOperatorExpressionWithArrayRhs(SemanticContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new InvalidOperationException("Expected an ArrayNode on the right-hand side of the operator expression, but found a different type.");
        }

        var op = node.Lhs.Value;
        var innerExpressions = new List<ExpressionNode>();

        foreach (var item in array.Values)
        {
            if (item is not LiteralNode)
            {
                throw new SemanticException("Error in query: All items in the list after the operator '" + node.Lhs.Value + "' must be values (literals). Found an invalid structure.", context);
            }

            var _lhs = new LhsNode(op);
            var _rhs = new RhsNode(item);

            innerExpressions.Add(new(_lhs, _rhs));
        }

        var innerScopes = innerExpressions
            .Transform(x => new ObjectNode(x))
            .ToArray();

        var anyLhs = new LhsNode(HelperTools.Stringify(Operator.Any));
        var anyRhs = new RhsNode(new ArrayNode(innerScopes));

        return Visit(context, new ExpressionNode(anyLhs, anyRhs));
    }

    /// <summary>
    /// Rewrites an iterator expression where the right-hand side (RHS) is a scope definition node.
    /// </summary>
    /// <remarks>
    /// This method modifies iterator expressions to ensure that the RHS is always an array node,
    /// even when originally defined as a single scope.
    /// <br/>
    /// Transformation example: <br/>
    /// Before: { "$any": { ... } } <br/>
    /// After:  { "$any": [{ ... }] }
    /// </remarks>
    /// <param name="node">The ExpressionNode representing an iterator expression.</param>
    /// <returns>The rewritten ExpressionNode with the RHS as an ArrayNode.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the RHS of the node is not a ScopeDefinitionNode.</exception>
    [return: NotNullIfNotNull("node")]
    private ExpressionNode? RewriteIteratorExpressionWithScopeRhs(SemanticContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not ObjectNode scope)
        {
            throw new InvalidOperationException("Error in query: For the iterator expression '" + node.Lhs.Value + "', the right-hand side must be a valid scope definition (object).");
        }
        throw new NotImplementedException();
        //var array = new ArrayNode(scope);
        //var rhs = new RhsNode(array);

        //return Visit(context, new ExpressionNode(node.Lhs, rhs));
    }

    /// <summary>
    /// Rewrites a member expression with a literal on the right-hand side.
    /// </summary>
    /// <remarks>
    /// Transformation example: <br/>
    /// Before: { "prop": "value" } <br/>
    /// After:  { "prop": { "$equals": "value" } }
    /// </remarks>
    /// <param name="node">The ExpressionNode to rewrite.</param>
    /// <returns>The rewritten node.</returns>
    [return: NotNullIfNotNull("node")]
    private ExpressionNode? RewriteMemberExpressionWithLiteralRhs(SemanticContext context, ExpressionNode node)
    {
        if (node.Rhs.Value is not LiteralNode literal)
        {
            throw new InvalidOperationException("Error in query: The right-hand side of the member expression '" + node.Lhs.Value + "' must be a literal value.");
        }

        var innerLhs = new LhsNode(HelperTools.Stringify(Operator.Equals));
        var innerRhs = new RhsNode(literal);
        var innerExpression = new ExpressionNode(innerLhs, innerRhs);
        var innerScope = new ObjectNode(new[] { innerExpression });
        var lhs = node.Lhs;
        var rhs = new RhsNode(innerScope);

        return Visit(context, new ExpressionNode(lhs, rhs));
    }

    /// <summary>
    /// Rewrites a member expression with an array on the right-hand side.
    /// </summary>
    /// <remarks>
    /// Transformation example: <br/>
    /// Before: { "prop": ["foo", "bar"] } <br/>
    /// After:  { "prop": { "$any": [{ "$equals": "foo" }, { "$equals": "bar" }] } }
    /// </remarks>
    /// <param name="node">The ExpressionNode to rewrite.</param>
    /// <returns>The rewritten node.</returns>
    [return: NotNullIfNotNull("node")]
    private ExpressionNode? RewriteMemberExpressionWithArrayRhs(SemanticContext context, ExpressionNode node)
    {
        //*
        // ex:
        // { prop: ["foo", "bar"] } =>
        // { prop: { $any: [{ $equals: "foo" }, { $equals: "bar" }] } }
        //*
        if (node.Rhs.Value is not ArrayNode array)
        {
            throw new InvalidOperationException("Error in query: Expected a list of values (array) on the right-hand side of the property '" + node.Lhs.Value + "', but found a different type.");
        }

        var expressions = new List<ExpressionNode>();

        foreach (var item in array.Values)
        {
            if (item is not LiteralNode && item is not ObjectNode)
            {
                throw new SemanticException("Error in query: Each item in the list for property '" + node.Lhs.Value + "' must be a value or a nested expression. Found an unsupported structure.", context);
            }

            var _lhs = new LhsNode(HelperTools.Stringify(Operator.Equals));
            var _rhs = new RhsNode(item);

            expressions.Add(new(_lhs, _rhs));
        }

        var scopes = expressions
            .Transform(x => new ObjectNode(new[] { x }))
            .ToArray();

        var anyExpressionLhs = new LhsNode(HelperTools.Stringify(Operator.Any));
        var anyExpressionRhs = new RhsNode(new ArrayNode(scopes));
        var anyExpression = new ExpressionNode(anyExpressionLhs, anyExpressionRhs);

        var memberAccessLhs = node.Lhs;
        var memberAccessRhsExpressions = new ExpressionNode[] { anyExpression };
        var memberAccessRhs = new RhsNode(new ObjectNode(memberAccessRhsExpressions));
        var memberExpression = new ExpressionNode(memberAccessLhs, memberAccessRhs);

        return Visit(context, memberExpression);
    }

}
