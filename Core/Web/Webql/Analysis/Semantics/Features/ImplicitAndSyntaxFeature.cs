using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.SyntaxFeatures;

/// <summary>
/// Implements the implicit "$and" syntax for WebQL analysis. <br/>
/// This feature enables the interpretation and transformation of query documents, <br/>
/// where the document's properties are inferred as "$equals" operators and combined <br/>
/// with logical "$and" operators. This transformation facilitates the handling of complex <br/>
/// query conditions and logical expressions in WebQL.
/// </summary>
internal class ImplicitAndSyntaxFeature : SemanticsVisitor
{
    /// <summary>
    /// Visits an ObjectNode and applies relational operators semantics.
    /// </summary>
    /// <param name="context">The current semantic context.</param>
    /// <param name="node">The ObjectNode to visit.</param>
    /// <returns>A Node transformed according to relational operators semantics.</returns>
    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, ObjectNode node)
    {
        if (!context.EnableImplicitAndSyntax)
        {
            return node;
        }

        var expressions = new List<ExpressionNode>();
        var objects = new List<ObjectNode>();

        foreach (var item in node)
        {
            var visitedItem = Visit(context, item).As<ExpressionNode>();

            expressions.Add(visitedItem);

            if (ExpressionEvaluatesToBool(visitedItem) && context.EnableNavigation)
            {
                objects.Add(new(expressions));
                expressions = new List<ExpressionNode>();
            }
        }

        if (objects.Count <= 1)
        {
            return new ObjectNode(objects.SelectMany(x => x.Expressions).Concat(expressions));
        }

        var lhs = new LhsNode(HelperTools.Stringify(Operator.And));
        var rhs = new RhsNode(new ArrayNode(objects));
        var expression = new ExpressionNode(lhs, rhs);

        return new ObjectNode(expression);
    }

    /// <summary>
    /// Determines if an expression node evaluates to a boolean value.
    /// </summary>
    /// <param name="node">The ExpressionNode to evaluate.</param>
    /// <returns>True if the node evaluates to a boolean; otherwise, false.</returns>
    private bool ExpressionEvaluatesToBool(ExpressionNode node)
    {
        var lastExpression = null as ExpressionNode;
        var workingExpression = node;

        while (true)
        {
            if(workingExpression.Rhs.Value is not ObjectNode objectNode)
            {
                break;
            }

            if (objectNode.IsEmpty())
            {
                break;
            }

            lastExpression = objectNode.Last();
            workingExpression = objectNode.Last();
        }

        if(lastExpression == null)
        {
            return false;
        }

        if(lastExpression.Lhs.IsReference)
        {
            return false;
        }

        return HelperTools.OperatorEvaluatesToBool(HelperTools.ParseOperatorString(lastExpression.Lhs.Value));
    }
}
