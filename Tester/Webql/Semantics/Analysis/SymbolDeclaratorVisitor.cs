using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a visitor for declaring symbols in the Webql syntax tree.
/// </summary>
public class SymbolDeclaratorVisitor : SyntaxTreeVisitor
{
    private SemanticContext SemanticContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolDeclaratorVisitor"/> class.
    /// </summary>
    /// <param name="context">The root semantic context.</param>
    public SymbolDeclaratorVisitor(SemanticContext context)
    {
        SemanticContext = context;
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
            DeclareRootSymbols(node);
        }

        else if (node is WebqlScopeAccessExpression scopeAccessExpression)
        {
            DeclareScopeAccessSymbols(scopeAccessExpression);
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
    private void DeclareRootSymbols(WebqlSyntaxNode node)
    {
        var context = node.GetSemanticContext();

        context.SetLeftHandSideSymbol(SemanticContext.CompilationContext.EntityQueryableType);
    }

    /// <summary>
    /// Declares the symbols for the scope access expression node.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression node.</param>
    private void DeclareScopeAccessSymbols(WebqlScopeAccessExpression scopeAccessExpression)
    {
        var context = scopeAccessExpression.GetSemanticContext();
        var subContext = scopeAccessExpression.Expression.GetSemanticContext();
        var propertyName = scopeAccessExpression.Identifier;
        var normalizedPropertyName = propertyName.ToLower();

        var lhsType = subContext.GetLeftHandSideType();
        var lhsProperties = lhsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var accessedProperty = lhsProperties
            .FirstOrDefault(x => x.Name.ToLower() == normalizedPropertyName);

        if (accessedProperty is null)
        {
            throw new SemanticException($"Property '{propertyName}' not found on type '{lhsType.Name}'.", scopeAccessExpression);
        }
            
        subContext.SetLeftHandSideSymbol(accessedProperty.PropertyType);

        foreach (var property in lhsProperties)
        {
            var symbol = new ScopePropertySymbol(
                identifier: property.Name,
                type: property.PropertyType
            );

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

