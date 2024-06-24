using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Exceptions;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class QueryTranslator
{
    public static Expression TranslateQuery(TranslationContext context, WebqlQuery node)
    {
        /*
         * Outputs a lambda expression that executes the query.
         */
        var semanticContext = node.GetSemanticContext();
        var lhsSemantics = semanticContext.GetLeftHandSideSymbol();

        var translationContext = node.GetTranslationContext();

        var parameterType = lhsSemantics.Type;
        var parameterName = lhsSemantics.Identifier;
        var parameter = Expression.Parameter(parameterType, parameterName);
        parameter = translationContext.GetLeftHandSideExpression<ParameterExpression>(); 

        if (node.Expression is null)
        {
            throw new TranslationException("Query expression is null.", context);
        }

        var body = ExpressionTranslator.TranslateExpression(context, node.Expression);

        throw new NotImplementedException();
    }
}

public class TranslatorContextBinderAnalyzer : SyntaxTreeAnalyzer
{
    private Stack<TranslationContext> ContextStack { get; }

    public TranslatorContextBinderAnalyzer(TranslationContext context)
    {
        ContextStack = new Stack<TranslationContext>();
        ContextStack.Push(context);
    }

    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        var localContext = ContextStack.Peek();
        var childContext = ContextStack.Peek();

        if (node.IsScopeSource())
        {
            childContext = localContext.CreateSubContext();
        }

        node.AddTranslationContext(localContext);

        ContextStack.Push(childContext);
        base.Analyze(node);
        ContextStack.Pop();
    }

}

/// <summary>
/// Represents a visitor for declaring symbols in the Webql syntax tree.
/// </summary>
public class ExpressionDeclaratorVisitor : SyntaxTreeVisitor
{
    private TranslationContext TranslationContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolDeclaratorVisitor"/> class.
    /// </summary>
    /// <param name="context">The root semantic context.</param>
    public ExpressionDeclaratorVisitor(TranslationContext context)
    {
        TranslationContext = context;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull("node")]
    public override WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return null;
        }

        if (node.IsRoot())
        {
            DeclareRootExpressions(node);
        }

        else if (node is WebqlScopeAccessExpression scopeAccessExpression)
        {
            DeclareScopeAccessExpressions(scopeAccessExpression);
        }

        else if (node is WebqlOperationExpression operationExpression)
        {
            DeclareOperationSymbols(operationExpression);
        }

        return base.Visit(node);
    }

    /// <summary>
    /// Declares the symbols for the root node.
    /// </summary>
    /// <param name="node">The root node.</param>
    private void DeclareRootExpressions(WebqlSyntaxNode node)
    {
        var context = node.GetTranslationContext();
        var type = TranslationContext.CompilationContext.EntityQueryableType;
        var name = "<root>";
        var expression = Expression.Parameter(type, name);

        context.SetLeftHandSideExpression(expression);
    }

    /// <summary>
    /// Declares the symbols for the scope access expression node.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression node.</param>
    private void DeclareScopeAccessExpressions(WebqlScopeAccessExpression scopeAccessExpression)
    {
        var context = scopeAccessExpression.GetTranslationContext();
        var subContext = scopeAccessExpression.Expression.GetTranslationContext();

        var propertyName = scopeAccessExpression.Identifier;
        var normalizedPropertyName = propertyName.ToLower();

        var lhsType = subContext.GetLeftHandSideExpressionType();
        var lhsProperties = lhsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var accessedProperty = lhsProperties
            .FirstOrDefault(x => x.Name.ToLower() == normalizedPropertyName);

        if (accessedProperty is null)
        {
            throw new TranslationException($"Property '{propertyName}' not found on type '{lhsType.Name}'.", context);
        }

        var propertyExpression = Expression.Property(subContext.GetLeftHandSideExpression(), accessedProperty);

        subContext.SetLeftHandSideExpression(propertyExpression);

        foreach (var property in lhsProperties)
        {
            var symbol = new ScopePropertySymbol(
                identifier: property.Name,
                type: property.PropertyType
            );

            var _propertyExpression = Expression.Property(subContext.GetLeftHandSideExpression(), property);

            subContext.AddSymbol(symbol);
        }
    }

    /// <summary>
    /// Declares the symbols for the operation expression node.
    /// </summary>
    /// <param name="operationExpression">The operation expression node.</param>
    private void DeclareOperationSymbols(WebqlOperationExpression operationExpression)
    {
        var operatorCategory = operationExpression.GetOperatorCategory();

        var operatorChangesLhs = false
            || operatorCategory == WebqlOperatorCategory.CollectionManipulation
            || operatorCategory == WebqlOperatorCategory.CollectionAggregation;

        if (!operatorChangesLhs)
        {
            return;
        }

        var context = operationExpression.GetSemanticContext();
        var lhsType = context.GetLeftHandSideType();

        if (lhsType.IsNotQueryable())
        {
            throw new SemanticException("Left-hand side type must be a queryable type.", operationExpression);
        }

        var elementType = lhsType.TryGetQueryableElementType();

        if (elementType is null)
        {
            throw new InvalidOperationException("Element type not found.");
        }

        foreach (var operand in operationExpression.Operands)
        {
            operand.GetSemanticContext().SetLeftHandSideSymbol(elementType);
        }
    }
}


