using Aidan.Core;
using System.Diagnostics.CodeAnalysis;

namespace Aidan.Webql.Analysis.SyntaxFeatures;

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
    protected override Node Visit(SemanticContextOld context, ObjectNode node)
    {
        //if (!context.EnableImplicitAndSyntax)
        //{
        //    return node;
        //}

        //var collectedExpressions = new List<ExpressionNode>();
        //var collectedObjects = new List<ObjectNode>();

        //foreach (var item in node.Expressions)
        //{
        //    var visited = Visit(context, item);

        //    collectedExpressions.Add(visited);

        //    if (ExpressionEvaluatesToBool(context, visited))
        //    {
        //        collectedObjects.Add(new ObjectNode(collectedExpressions));
        //        collectedExpressions.Clear();
        //    }
        //}

        //if (collectedObjects.Count <= 1)
        //{
        //    return node;
        //}

        //var lhs = new LhsNode(WebqlHelper.Stringify(Operator.And));
        //var rhs = new RhsNode(new ArrayNode(collectedObjects));
        //var expression = new ExpressionNode(lhs, rhs);

        //return new ObjectNode(expression);

        //*
        //*
        //*

        if (!context.EnableImplicitAndSyntax)
        {
            return node;
        }

        var expressions = new List<ExpressionNode>();
        var objects = new List<ObjectNode>();

        foreach (var item in node)
        {
            var visitedItem = Visit(context, item);

            expressions.Add(visitedItem);

            if (ExpressionEvaluatesToBool(context, visitedItem) && context.EnableNavigation)
            {
                objects.Add(new(expressions));
                expressions = new List<ExpressionNode>();
            }
        }

        if (objects.Count <= 1)
        {
            return new ObjectNode(objects.SelectMany(x => x.Expressions).Concat(expressions));
        }

        var lhs = new LhsNode(WebqlHelper.Stringify(OperatorOld.And));
        var rhs = new RhsNode(new ArrayNode(objects));
        var expression = new ExpressionNode(lhs, rhs);

        return new ObjectNode(expression);
    }

    /// <summary>
    /// Determines if an expression node evaluates to a boolean value.
    /// </summary>
    /// <param name="context">The current semantic context.</param>
    /// <param name="node">The ExpressionNode to evaluate.</param>
    /// <returns>True if the node evaluates to a boolean; otherwise, false.</returns>
    private bool ExpressionEvaluatesToBool(SemanticContextOld context, ExpressionNode node)
    {
        ExpressionNode? expression = node;

        if (expression.Lhs.IsOperator)
        {
            var op = ParseOperatorString(context, expression.Lhs.Value);
            var operatorType = WebqlHelper.GetOperatorType(op);
            var operatorIsQueryable = operatorType == OperatorTypeOld.Queryable;

            if(operatorIsQueryable)
            {
                return false;
            }
            else
            {
                if (expression.Rhs.Value is ObjectNode objectNode)
                {
                    expression = objectNode.LastOrDefault();
                }
            }
        }
        else
        {
            if (expression.Rhs.Value is ObjectNode objectNode)
            {
                expression = objectNode.LastOrDefault();
            }
        }

        if (expression == null)
        {
            return false;
        }

        return WebqlHelper.OperatorEvaluatesToBool(ParseOperatorString(context, expression.Lhs.Value));
    }
}
