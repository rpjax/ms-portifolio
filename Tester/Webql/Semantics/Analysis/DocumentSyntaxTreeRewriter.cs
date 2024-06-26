using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

public class DocumentSyntaxTreeRewriter : SyntaxTreeRewriter
{
    private Stack<WebqlExpression> LhsStack { get; } = new();

    public DocumentSyntaxTreeRewriter()
    {

    }

    public override WebqlExpression VisitScopeAccessExpression(WebqlScopeAccessExpression node)
    {
        var generatedLhs = new WebqlReferenceExpression(
            metadata: node.Metadata,
            attributes: null,
            identifier: SemanticContext.LeftHandSideId
        );

        LhsStack.Push(generatedLhs);
        var visitedNode = base.VisitScopeAccessExpression(node);
        LhsStack.Pop();

        return visitedNode;
    }

    public override WebqlExpression VisitOperationExpression(WebqlOperationExpression node)
    {
        var isLinqQueryableMethodCall = node.IsLinqQueryableMethodCallOperator();

        if (isLinqQueryableMethodCall)
        {
            var generatedLhs = new WebqlReferenceExpression(
                metadata: node.Metadata,
                attributes: null,
                identifier: SemanticContext.LeftHandSideId
            );

            LhsStack.Push(generatedLhs);
            var visitedNode = base.VisitOperationExpression(node);
            LhsStack.Pop();

            return visitedNode;
        }

        var lhs = VisitExpression(LhsStack.Peek());
        var rhs = VisitExpression(node.Operands[0]);
        var operands = new WebqlExpression[] { lhs, rhs };

        return new WebqlOperationExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            @operator: node.Operator,
            operands: operands
        );
    }
}
