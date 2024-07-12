using System.Runtime.CompilerServices;
using Webql.Core;
using Webql.Core.Analysis;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;
using Webql.Semantics.Attributes;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Scope;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Extensions;

/*
 * This extension class provides the main API for semantic related operations on the syntax tree.
 */

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeSemanticExtensions
{
    /*
     * Scope related extensions
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasScopeAttribute(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.ScopeAttribute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlScope GetScope(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if (!current.HasScopeAttribute())
            {
                current = current.Parent;
                continue;
            }

            if (!current.TryGetAttribute<WebqlScope>(AstSemanticAttributes.ScopeAttribute, out var scope))
            {
                throw new InvalidOperationException();
            }

            return scope;
        }

        throw new InvalidOperationException("Scope not found");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindScope(this WebqlSyntaxNode node, WebqlScope scope, bool enableOverride = false)
    {
        if (enableOverride && node.HasAttribute(AstSemanticAttributes.ScopeAttribute))
        {
            node.RemoveAttribute(AstSemanticAttributes.ScopeAttribute);
        }

        node.AddAttribute(AstSemanticAttributes.ScopeAttribute, scope);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlScopeType GetScopeType(this WebqlSyntaxNode node)
    {
        return node.GetScope().Type;
    }

    /*
     * Semantics related extensions
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISemantics GetSemantics(this WebqlSyntaxNode node)
    {
        return SemanticAnalyzer.CreateSemantics(node.GetCompilationContext(), node);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSemantics GetSemantics<TSemantics>(this WebqlSyntaxNode node) where TSemantics : ISemantics
    {
        var compilationContext = node.GetCompilationContext();
        var semantics = SemanticAnalyzer.CreateSemantics(compilationContext, node);

        if(semantics is not TSemantics cast)
        {
            throw new InvalidOperationException();
        }

        return cast;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindSemantics(this WebqlSyntaxNode node, ISemantics semantics)
    {
        node.AddAttribute(AstSemanticAttributes.SemanticsAttribute, semantics);
    }

    /*
     * Symbols related extensions
     */

    public static ISymbol? TryResolveSymbol(this WebqlSyntaxNode node, string identifier)
    {
        var scope = node.GetScope();
        var symbol = scope.ResolveSymbol(identifier);

        return symbol;
    }

    public static TSymbol? TryResolveSymbol<TSymbol>(this WebqlSyntaxNode node, string identifier) where TSymbol : class, ISymbol
    {
        var symbol = node.TryResolveSymbol(identifier);

        if (symbol is null)
        {
            return null;
        }

        if (symbol is not TSymbol typedSymbol)
        {
            throw new InvalidOperationException();
        }

        return typedSymbol;
    }

    public static ISymbol ResolveSymbol(this WebqlSyntaxNode node, string identifier)
    {
        var symbol = node.TryResolveSymbol(identifier);

        if (symbol is null)
        {
            throw node.CreateSymbolNotFoundException(identifier);
        }

        return symbol;
    }

    public static TSymbol ResolveSymbol<TSymbol>(this WebqlSyntaxNode node, string identifier) where TSymbol : class, ISymbol
    {
        var symbol = node.ResolveSymbol(identifier);

        if (symbol is not TSymbol typedSymbol)
        {
            throw new InvalidOperationException();
        }

        return typedSymbol;
    }

    /*
     * Source symbol related extensions
     */

    public static SourceSymbol GetSourceSymbol(this WebqlSyntaxNode node)
    {
        var scope = node.GetScope();    
        var symbol = scope.ResolveSymbol<SourceSymbol>(WebqlAstSymbols.SourceIdentifier);

        if(symbol is null)
        {
            throw node.CreateSymbolNotFoundException(WebqlAstSymbols.SourceIdentifier);
        }

        return symbol;
    }

    public static void DeclareSourceSymbol(this WebqlSyntaxNode node, Type type)
    {
        var scope = node.GetScope();

        var symbol = new SourceSymbol(
            identifier: WebqlAstSymbols.SourceIdentifier,
            type: type
        );

        if(scope.ContainsSymbol(symbol.Identifier, useParentScope: false))
        {
            node.CreateSymbolAlreadyDeclaredException(symbol.Identifier);
        }

        scope.DeclareSymbol(symbol);
    }

    /*
     * Element symbol related extensions
     */

    public static ParameterSymbol GetElementSymbol(this WebqlSyntaxNode node)
    {
        var scope = node.GetScope();
        var symbol = scope.ResolveSymbol<ParameterSymbol>(WebqlAstSymbols.ElementIdentifier);

        if (symbol is null)
        {
            throw node.CreateSymbolNotFoundException(WebqlAstSymbols.ElementIdentifier);
        }

        return symbol;
    }

    public static void DeclareElementSymbol(this WebqlSyntaxNode node, Type type)
    {
        var scope = node.GetScope();

        var symbol = new ParameterSymbol(
            identifier: WebqlAstSymbols.ElementIdentifier,
            type: type
        );

        if(scope.ContainsSymbol(symbol.Identifier, useParentScope: false))
        {
            node.CreateSymbolAlreadyDeclaredException(symbol.Identifier);
        }

        scope.DeclareSymbol(symbol);
    }

    /*
     * Expression related extensions
     */

    public static void EnsureOperandCount(this WebqlOperationExpression node, int expectedCount)
    {
        var actualCount = node.Operands.Length;

        if (actualCount != expectedCount)
        {
            throw node.CreateInvalidOperandCountException(expectedCount, actualCount);
        }
    }

    public static void EnsureAtLeastOneOperand(this WebqlOperationExpression node)
    {
        var actualCount = node.Operands.Length;

        if (actualCount == 0)
        {
            throw new SemanticException($"Operator '{node.Operator}' expects at least one operand, but none were provided", node);
        }
    }

    public static void EnsureIsQueryable(this WebqlExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var type = semantics.Type;

        if (type.IsNotQueryable())
        {
            throw node.CreateExpressionIsNotQuryableException(type);
        }
    }

    public static Type GetExpressionType(this WebqlExpression node)
    {
        return node.GetSemantics<IExpressionSemantics>().Type;
    }

    public static IExpressionSemantics GetExpressionSemantics(this WebqlExpression node)
    {
        return node.GetSemantics<IExpressionSemantics>();
    }

    /*
     * 
     */

    /*
     * Generic helpers
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetSemanticIdentifier(this WebqlSyntaxNode node)
    {
        return "not implemented yet";
    }

    public static Type GetQueryableType(this WebqlSyntaxNode node, WebqlCompilationContext context)
    {
        if (node.IsInRootAggregationScope())
        {
            return context.RootQueryableType;
        }

        return context.QueryableType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("This has no semantical meaning anymore. Use the ScopeType instead.")]
    public static bool IsScopeSource(this WebqlSyntaxNode node)
    {
        if (node is not WebqlOperationExpression operationExpression)
        {
            return false;
        }

        var @operator = operationExpression.Operator;

        return WebqlOperatorAnalyzer.IsCollectionOperator(@operator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRoot(this WebqlSyntaxNode node)
    {
        return node.Parent is null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotRoot(this WebqlSyntaxNode node)
    {
        return !node.IsRoot();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollectionOperator(this WebqlSyntaxNode node)
    {
        if (node is not WebqlOperationExpression operationExpression)
        {
            return false;
        }

        return WebqlOperatorAnalyzer.IsCollectionOperator(operationExpression.Operator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAggregationScope(this WebqlSyntaxNode node)
    {
        return node.GetScopeType() is WebqlScopeType.Aggregation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRootAggregationScope(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if (current.IsRoot())
            {
                return true;
            }

            if (current.IsCollectionOperator())
            {
                return false;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException();
    }

    /*
     * Exceptions extensions
     * NOTE: The semantic errors are informed to the user by throwing exceptions. In the future this may change.
     */

    public static SemanticException CreateSymbolNotFoundException(
        this WebqlSyntaxNode node,
        string identifier)
    {
        return new SemanticException($"Symbol '{identifier}' not found", node);
    }

    public static SemanticException CreatePropertyNotFoundException(
        this WebqlSyntaxNode node,
        Type type,
        string propertyName)
    {
        return new SemanticException($"Property '{propertyName}' not found in type '{type.FullName}'", node);
    }

    public static SemanticException CreateOperatorIncompatibleTypeException(
        this WebqlSyntaxNode node,
        Type leftType,
        Type rightType)
    {
        return new SemanticException($@"
Error: Operator Incompatible Type Exception

Description:
The operation you are attempting to perform involves incompatible types.
This error occurs when the types of the operands do not match the expected types
for the operation being executed. Specifically, the operator cannot be applied to 
operands of the given types.

For example, attempting to add a string to an integer or comparing a boolean 
with a string will result in this exception.

Details:
- Left Operand Type: {leftType.FullName}
- Right Operand Type: {rightType.FullName}

Resolution:
Ensure that the operands used in the operation are of compatible types. 
Refer to the documentation or type specifications for the expected operand types 
for the operator being used.
", node);
    }

    public static SemanticException CreateInvalidOperandCountException(
        this WebqlOperationExpression node,
        int expectedCount,
        int actualCount)
    {
        return new SemanticException($"Operator '{node.Operator}' expects {expectedCount} operands, but {actualCount} were provided", node);
    }

    public static SemanticException CreateExpressionIsNotQuryableException(
        this WebqlExpression node,
        Type type)
    {
        return new SemanticException($"Expression is not a queryable type. Type: {type.FullName}", node);
    }

    public static SemanticException CreateSymbolAlreadyDeclaredException(
        this WebqlSyntaxNode node,
        string identifier)
    {
        return new SemanticException($"Symbol '{identifier}' already declared in the current scope", node);
    }

}
