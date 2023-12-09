using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.SyntaxFeatures;

/// <summary>
/// Enalbes the filter syntax for "query documents" where the documents properties are infered "$equals" operators, 
/// combined by logical "$and" operators.
/// </summary>
internal class RelationalOperatorsSyntaxFeature : SemanticsVisitor
{
    [return: NotNullIfNotNull("node")]
    protected override Node? Visit(SemanticContext context, ObjectNode node)
    {
        var expressions = new List<ExpressionNode>();
        var objects = new List<ObjectNode>();

        foreach (var item in node)
        {
            var visitedItem = Visit(context, item).As<ExpressionNode>();

            expressions.Add(visitedItem);

            if (ExpressionEvaluatesToBool(visitedItem))
            {
                objects.Add(new(expressions));
                expressions = new();
            }
        }

        if(objects.Count <= 1)
        {
            return new ObjectNode(objects.SelectMany(x => x.Expressions).Concat(expressions));
        }

        var lhs = new LhsNode(HelperTools.StringifyOperator(OperatorV2.And));
        var rhs = new RhsNode(new ArrayNode(objects));
        var expression = new ExpressionNode(lhs, rhs);

        return new ObjectNode(expression);
    }

    private bool ExpressionEvaluatesToBool(ExpressionNode node)
    {
        while (true)
        {
            var lhs = node.Lhs;
            var rhs = node.Rhs.Value;
            var isMemberAccess = !lhs.IsOperator;

            if (isMemberAccess)
            {
                if(rhs is not ObjectNode objectNode)
                {
                    return false;
                }
                if (objectNode.IsEmpty())
                {
                    return false;
                }

                node = objectNode.Last();
                continue;
            }

            var op = HelperTools.ParseOperatorString(lhs.Value);
            var opType = HelperTools.GetOperatorType(op);

            return opType == OperatorType.Relational || opType == OperatorType.Logical;
        }
    }

}
