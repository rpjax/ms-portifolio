using System.Linq.Expressions;
using System.Reflection;
using Webql.Core;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Analysis;

public class ExpressionDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    private TranslationContext TranslationContext { get; }

    public ExpressionDeclaratorAnalyzer(TranslationContext context)
    {
        TranslationContext = context;
    }

    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        if (node.IsRoot())
        {
            DeclareRootExpressions(node);
        }

        //else if (node is WebqlScopeAccessExpression scopeAccessExpression)
        //{
        //    DeclareScopeAccessExpressions(scopeAccessExpression);
        //}

        else if (node is WebqlOperationExpression operationExpression)
        {
            DeclareOperationExpressions(operationExpression);
        }

        base.Analyze(node);
    }

    private void DeclareRootExpressions(WebqlSyntaxNode node)
    {
        var context = node.GetTranslationContext();
        var type = TranslationContext.CompilationContext.EntityQueryableType;
        var name = "<root>";
        var expression = Expression.Parameter(type, name);

        context.SetLeftHandSideExpression(expression);
    }

    private void DeclareOperationExpressions(WebqlOperationExpression node)
    {
        var operatorCategory = node.GetOperatorCategory();

        var isLinqQueryableMethodCall = node.IsLinqQueryableMethodCallOperator();

        if (!isLinqQueryableMethodCall)
        {
            return;
        }

        var semanticContext = node.GetSemanticContext();

        var queryableType = semanticContext.GetLeftHandSideType();
        var elementType = queryableType.GetQueryableElementType();
        var elementProperties = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name == "Length" || !p.DeclaringType?.Namespace?.StartsWith("System") == true)
            .ToArray()
            ;

        var elementExpression = Expression.Parameter(elementType, "<element>");

        var expressionDeclarations = elementProperties
           .Select(x => Expression.Property(elementExpression, x))
           .ToArray()
           ;

        foreach (var operand in node.Operands)
        {
            var translationContext = operand.GetTranslationContext();

            translationContext.SetLeftHandSideExpression(elementExpression);

            foreach (var expression in expressionDeclarations)
            {
                translationContext.AddExpression(expression.Member.Name, expression);
            }
        }
    }

    //private void DeclareScopeAccessExpressions(WebqlScopeAccessExpression node)
    //{
    //    var localTranslationContext = node.GetTranslationContext();
    //    var childTranslationContext = node.Expression.GetTranslationContext();

    //    var expressionId = node.Identifier;
    //    var normalizedExpressionId = IdentifierHelper.NormalizeIdentifier(expressionId);

    //    var referencedExpression = localTranslationContext.GetExpression(expressionId);

    //    var expressionType = referencedExpression.Type;
    //    var expressionProperties = expressionType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //        .Where(p => p.Name == "Length" || !p.DeclaringType?.Namespace?.StartsWith("System") == true)
    //        .ToArray()
    //        ;

    //    var expressionDeclarations = expressionProperties
    //        .Select(x => Expression.Property(referencedExpression, x))
    //        .ToArray()
    //        ;

    //    childTranslationContext.SetLeftHandSideExpression(referencedExpression);

    //    foreach (var expression in expressionDeclarations)
    //    {
    //        childTranslationContext.AddExpression(expression.Member.Name, expression);
    //    }
    //}
  
}

