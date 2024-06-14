using System.Diagnostics.CodeAnalysis;
using Webql.Components;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;
using Webql.Semantics.Components;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Tools;

public class TypeValidatorVisitor : SyntaxNodeVisitor
{
    private WebqlCompilationContext CompilationContext { get; }

    public TypeValidatorVisitor(WebqlCompilationContext compilationContext)
    {
        CompilationContext = compilationContext;
    }

    [return: NotNullIfNotNull("node")]
    public override WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        return base.Visit(node);
    }

    public override WebqlExpression VisitOperationExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.IsBinary())
        {
            var context = operationExpression.GetSemanticContext(); 
            var lhsSemantics = context.GetLeftHandSideSymbol();
            var rhsSemantics = operationExpression.Operands[0].GetSemantics<IExpressionSemantics>();

            if (lhsSemantics.Type != rhsSemantics.Type)
            {
                throw new Exception($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}");
            }

            Console.WriteLine();
        }   

        return base.VisitOperationExpression(operationExpression);
    }

}

