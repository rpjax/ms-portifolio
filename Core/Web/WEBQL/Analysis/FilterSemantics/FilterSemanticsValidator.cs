using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

public class FilterSemanticsValidatorVisitor : SemanticsVisitor
{
    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        node = base.Visit(context, node);

        if (node.Lhs.IsOperator)
        {
            return VisitOperatorExpression(context, node);
        }

        return node;
    }

    protected ExpressionNode VisitOperatorExpression(SemanticContext context, ExpressionNode node)
    {
        var operators = Enum.GetValues(typeof(Operator));
        var opFound = false;

        foreach (Operator op in operators)
        {
            if (HelperTools.Stringify(op) == node.Lhs.Value.ToCamelCase())
            {
                opFound = true;
            }
        }

        if (!opFound)
        {
            throw new SemanticException($"Operator '{node.Lhs.Value}' is not recognized or supported. Ensure the operator is valid in the current context.", context);
        }

        return node;
    }

}
