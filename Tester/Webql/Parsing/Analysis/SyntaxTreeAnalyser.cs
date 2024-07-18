﻿using Webql.Parsing.Ast;

namespace Webql.Parsing.Analysis;

/// <summary>
/// Represents a visitor for analyzing the syntax tree.
/// </summary>
public class SyntaxTreeAnalyzer : SyntaxTreeVisitor
{
    /// <summary>
    /// Executes the analysis of the syntax tree.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree to analyze.</param>
    public void ExecuteAnalysis(WebqlSyntaxNode syntaxTree)
    {
        Analyze(syntaxTree);
    }

    /// <summary>
    /// Analyzes the given syntax tree node.
    /// </summary>
    /// <param name="node">The syntax tree node to analyze.</param>
    protected virtual void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                AnalyzeQuery((WebqlQuery)node);
                return;

            case WebqlNodeType.Expression:
                AnalyzeExpression((WebqlExpression)node);
                return;

            case WebqlNodeType.AnonymousObjectProperty:
                AnalyzeAnonymousObjectProperty((WebqlAnonymousObjectProperty)node);
                return;

            default:
                throw new InvalidOperationException("Invalid node type.");
        }
    }

    /// <summary>
    /// Analyzes the given query.
    /// </summary>
    /// <param name="query">The query to analyze.</param>
    protected virtual void AnalyzeQuery(WebqlQuery query)
    {
        Analyze(query.Expression);
    }

    /// <summary>
    /// Analyzes the given expression.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    protected virtual void AnalyzeExpression(WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                AnalyzeLiteralExpression((WebqlLiteralExpression)expression);
                return;

            case WebqlExpressionType.Reference:
                AnalyzeReferenceExpression((WebqlReferenceExpression)expression);
                return;

            case WebqlExpressionType.MemberAccess:
                AnalyzeMemberAccessExpression((WebqlMemberAccessExpression)expression);
                return;

            case WebqlExpressionType.TemporaryDeclaration:
                AnalyzeTemporaryDeclarationExpression((WebqlTemporaryDeclarationExpression)expression);
                return;

            case WebqlExpressionType.Block:
                AnalyzeBlockExpression((WebqlBlockExpression)expression);
                return;

            case WebqlExpressionType.Operation:
                AnalyzeOperationExpression((WebqlOperationExpression)expression);
                return;

            case WebqlExpressionType.TypeConversion:
                AnalyzeTypeConversionExpression((WebqlTypeConversionExpression)expression);
                return;

            case WebqlExpressionType.AnonymousObject:
                AnalyzeAnonymousObjectExpression((WebqlAnonymousObjectExpression)expression);
                return;

            default:
                throw new InvalidOperationException("Invalid operand type.");
        }
    }

    /// <summary>
    /// Analyzes the given literal expression.
    /// </summary>
    /// <param name="expression">The literal expression to analyze.</param>
    protected virtual void AnalyzeLiteralExpression(WebqlLiteralExpression expression)
    {
        return;
    }

    /// <summary>
    /// Analyzes the given reference expression.
    /// </summary>
    /// <param name="expression">The reference expression to analyze.</param>
    protected virtual void AnalyzeReferenceExpression(WebqlReferenceExpression expression)
    {
        return;
    }

    /// <summary>
    /// Analyzes the given scope access expression.
    /// </summary>
    /// <param name="expression">The scope access expression to analyze.</param>
    protected virtual void AnalyzeMemberAccessExpression(WebqlMemberAccessExpression expression)
    {
        Analyze(expression.Expression);
    }

    /// <summary>
    /// Analyzes the given temporary declaration expression.
    /// </summary>
    /// <param name="expression">The temporary declaration expression to analyze.</param>
    protected virtual void AnalyzeTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression expression)
    {
        Analyze(expression.Value);
    }

    /// <summary>
    /// Analyzes the given block expression.
    /// </summary>
    /// <param name="expression">The block expression to analyze.</param>
    protected virtual void AnalyzeBlockExpression(WebqlBlockExpression expression)
    {
        foreach (var childExpression in expression.Expressions)
        {
            Analyze(childExpression);
        }
    }

    /// <summary>
    /// Analyzes the given operation expression.
    /// </summary>
    /// <param name="expression">The operation expression to analyze.</param>
    protected virtual void AnalyzeOperationExpression(WebqlOperationExpression expression)
    {
        foreach (var operand in expression.Operands)
        {
            Analyze(operand);
        }
    }

    protected virtual void AnalyzeTypeConversionExpression(WebqlTypeConversionExpression expression)
    {
        Analyze(expression.Expression);
    }

    protected virtual void AnalyzeAnonymousObjectExpression(WebqlAnonymousObjectExpression expression)
    {
        foreach (var property in expression.Properties)
        {
            Analyze(property);
        }
    }

    protected virtual void AnalyzeAnonymousObjectProperty(WebqlAnonymousObjectProperty property)
    {
        Analyze(property.Value);
    }

}
