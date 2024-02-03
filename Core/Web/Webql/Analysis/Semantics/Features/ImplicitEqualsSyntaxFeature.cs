using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.SyntaxFeatures;

/// <summary>
/// A specialized semantics visitor that introduces implicit equality checks into the syntax tree. <br/>
/// This class extends the base SemanticsVisitor to transform certain node patterns into explicit equality checks, enhancing the expressiveness of the query syntax.
/// </summary>
internal class ImplicitEqualsSyntaxFeature : SemanticsVisitor
{
    /// <summary>
    /// Visits and potentially modifies an ExpressionNode based on the implicit equals syntax feature.
    /// </summary>
    /// <param name="context">The semantic context in which the node is being visited.</param>
    /// <param name="node">The ExpressionNode to visit.</param>
    /// <returns>An ExpressionNode modified to include implicit equality checks, if applicable.</returns>
    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        var subStack = $".{node.Lhs.Value}";
        var lhs = node.Lhs;
        var rhs = node.Rhs.Value;

        // Process if the left-hand side is an operator
        if (lhs.IsOperator)
        {
            return base.Visit(context, node);
        }

        // Handle navigation disabled case
        if (!context.EnableNavigation && node.Lhs.IsReference)
        {
            return new ExpressionNode(Visit(context, node.Lhs), Visit(context, node.Rhs));
        }

        // Update context based on the left-hand side reference
        if (node.Lhs.IsReference)
        {
            context = context.GetReferenceContext(node.Lhs.Value, subStack);
        }

        // Transform literal right-hand side into explicit equality check
        if (rhs is LiteralNode literal)
        {
            var childLhs = new LhsNode(HelperTools.Stringify(Operator.Equals));
            var childRhs = new RhsNode(literal);
            var childExpression = new ExpressionNode(childLhs, childRhs);
            var child = new ObjectNode(childExpression);

            return new ExpressionNode(lhs, new(child));
        }

        // Transform array right-hand side into a series of OR'ed equalities
        if (rhs is ArrayNode array)
        {
            var orLhs = new LhsNode(HelperTools.Stringify(Operator.Or));
            var orObjects = new List<ObjectNode>();

            foreach (var item in array)
            {
                var childLhs = new LhsNode(HelperTools.Stringify(Operator.Equals));
                var childRhs = new RhsNode(item);
                var expressions = new List<ExpressionNode>
                {
                    new(childLhs, childRhs)
                };
                var childObject = new ObjectNode(expressions);

                orObjects.Add(childObject);
            }

            var childArray = new ArrayNode(orObjects);
            var orExpression = new ExpressionNode(orLhs, new(childArray));
            var orObject = new ObjectNode(orExpression);

            var referenceLhs = node.Lhs;
            var referenceRhs = new RhsNode(orObject);
            var referenceExpression = new ExpressionNode(referenceLhs, referenceRhs);

            return referenceExpression;
        }

        // Fallback to base visit method
        return base.Visit(context, node);
    }
}
