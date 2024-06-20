﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Webql.Components;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;
using Webql.Semantics.Components;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Tools;

/// <summary>
/// Represents a visitor for declaring symbols in the Webql syntax tree.
/// </summary>
public class SymbolDeclaratorVisitor : SyntaxTreeVisitor
{
    private WebqlCompilationContext CompilationContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolDeclaratorVisitor"/> class.
    /// </summary>
    /// <param name="compilationContext">The compilation semanticContext.</param>
    public SymbolDeclaratorVisitor(WebqlCompilationContext compilationContext)
    {
        CompilationContext = compilationContext;
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

        context.SetLeftHandSideSymbol(CompilationContext.EntityQueryableType);
    }

    /// <summary>
    /// Declares the symbols for the scope access expression node.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression node.</param>
    private void DeclareScopeAccessSymbols(WebqlScopeAccessExpression scopeAccessExpression)
    {
        var context = scopeAccessExpression.GetSemanticContext();
        var subContext = scopeAccessExpression.Expression.GetSemanticContext();
        var accessedPropertyName = scopeAccessExpression.Identifier.ToLower();

        var lhsType = subContext.GetLeftHandSideType();
        var lhsProperties = lhsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var accessedProperty = lhsProperties
            .FirstOrDefault(x => x.Name.ToLower() == accessedPropertyName);

        if (accessedProperty is null)
        {
            throw new InvalidOperationException("Property not found on type.");
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

