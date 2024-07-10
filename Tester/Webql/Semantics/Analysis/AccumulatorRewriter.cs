using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Tester.Webql.Semantics.Analysis;

public class AccumulatorRewriter : SyntaxTreeRewriter
{
    public override WebqlExpression VisitOperationExpression(WebqlOperationExpression node)
    {
        if (node.Operator is not WebqlOperatorType.Aggregate)
        {
            return base.VisitOperationExpression(node);
        }

        foreach (var child in node.GetChildren())
        {
            child.GetScopeType();
        }

        return base.VisitOperationExpression(node);

    }
}
