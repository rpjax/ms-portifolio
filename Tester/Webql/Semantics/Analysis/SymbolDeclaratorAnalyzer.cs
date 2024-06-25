using System.Reflection;
using Webql.Core;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a visitor for declaring symbolDeclarations in the Webql syntax tree.
/// </summary>
public class SymbolDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    private SemanticContext SemanticContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolDeclaratorAnalyzer"/> class.
    /// </summary>
    /// <param name="context">The root semantic localContext.</param>
    public SymbolDeclaratorAnalyzer(SemanticContext context)
    {
        SemanticContext = context;
    }

    /// <inheritdoc/>
    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
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

        base.Analyze(node);
    }

    /// <summary>
    /// Declares the symbolDeclarations for the root node.
    /// </summary>
    /// <param name="node">The root node.</param>
    private void DeclareRootSymbols(WebqlSyntaxNode node)
    {
        var localContext = node.GetSemanticContext();
        var lhsType = SemanticContext.CompilationContext.EntityQueryableType;

        localContext.SetLeftHandSideSymbol(lhsType);
    }

    /// <summary>
    /// Declares the symbolDeclarations for the operation expression node.
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

        var localContext = operationExpression.GetSemanticContext();
        var lhsType = localContext.GetLeftHandSideType();

        if (lhsType.IsNotQueryable())
        {
            throw new SemanticException("Left-hand side lhsType must be a queryable lhsType.", operationExpression);
        }

        var lhsElementType = lhsType.GetQueryableElementType();
        var elementProperties = lhsElementType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name == "Length" || !p.DeclaringType?.Namespace?.StartsWith("System") == true)
            .ToArray()
            ;

        var symbolDeclarations = elementProperties
            .Select(x => new ScopePropertySymbol(x.Name, x.PropertyType))
            .ToArray()
            ;

        foreach (var operand in operationExpression.Operands)
        {
            var childContext = operand.GetSemanticContext();

            childContext.SetLeftHandSideSymbol(lhsElementType);
            
            foreach (var symbol in symbolDeclarations)
            {
                childContext.AddSymbol(symbol);
            }
        }
    }

    /// <summary>
    /// Declares the symbolDeclarations for the scope access expression node.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression node.</param>
    private void DeclareScopeAccessSymbols(WebqlScopeAccessExpression scopeAccessExpression)
    {
        var localContext = scopeAccessExpression.GetSemanticContext();
        var childContext = scopeAccessExpression.Expression.GetSemanticContext();

        var symbolId = scopeAccessExpression.Identifier;
        var normalizedSymbolId = IdentifierHelper.NormalizeIdentifier(symbolId);

        var referencedSymbol = localContext.TryGetSymbol(symbolId);

        if(referencedSymbol is null)
        {
            throw new SemanticException($"The referenced symbol '{symbolId}' was not found in the current context.", scopeAccessExpression);
        }

        var symbolType = referencedSymbol.Type;
        var symbolProperties = symbolType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name == "Length" || !p.DeclaringType?.Namespace?.StartsWith("System") == true)
            .ToArray()
            ;

        var symbolDeclarations = symbolProperties
            .Select(x => new ScopePropertySymbol(x.Name, x.PropertyType))
            .ToArray()
            ;

        childContext.SetLeftHandSideSymbol(symbolType);

        foreach (var symbol in symbolDeclarations)
        {
            childContext.AddSymbol(symbol);
        }
    }

}

