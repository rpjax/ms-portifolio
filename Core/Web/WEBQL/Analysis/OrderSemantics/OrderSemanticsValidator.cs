using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis;

public class OrderSemanticsValidator : SemanticsVisitor
{
    [return: NotNullIfNotNull("node")]
    protected override ExpressionNode? Visit(SemanticContext context, ExpressionNode node)
    {
        if (node.Lhs.IsOperator)
        {
            throw new SemanticException($"Ordering Error: Operators are not supported in ordering expressions. Found operator '{node.Lhs.Value}'.", context);
        }

        return base.Visit(context, node);
    }
}
