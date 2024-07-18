using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Definitions;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

public class ImplicitTypeConversionRewriter : SyntaxTreeRewriter
{
    public override WebqlExpression VisitOperationExpression(WebqlOperationExpression node)
    {

        if(!node.IsBinaryTypeCompatibleOperator())
        {
            return base.VisitOperationExpression(node);
        }

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();
        var rhsSemantics = rhs.GetSemantics<IExpressionSemantics>();

        if (!SemanticsTypeHelper.TypesAreCompatible(lhsSemantics.Type, rhsSemantics.Type))
        {
            return base.VisitOperationExpression(node);
        }

        if(lhsSemantics.Type == rhsSemantics.Type)
        {
            return base.VisitOperationExpression(node);
        }

        var conversionExpression = new WebqlTypeConversionExpression(
            metadata: rhs.Metadata,
            attributes: null,
            expression: rhs,
            targetType: lhsSemantics.Type
        );

        node = new WebqlOperationExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            @operator: node.Operator,
            operands: new[] { lhs, conversionExpression }
        );

        return base.VisitOperationExpression(node);
    }
}
