﻿using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Extensions;
using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Parsing.Tools;

/// <summary>
/// Provides methods to translate a CstRoot node into a WebqlQuery object.
/// </summary>
public static class WebqlAstBuilder
{
    /// <summary>
    /// Translates a CstRoot node into a WebqlQuery object.
    /// </summary>
    /// <param name="node">The CstRoot node to translate.</param>
    /// <returns>The translated WebqlQuery object.</returns>
    public static WebqlQuery TranslateQuery(CstRoot node)
    {
        if (node.Children.Length > 1)
        {
            throw new Exception("Invalid query");
        }
        if (node.Children.Length == 0)
        {
            return new WebqlQuery(null);
        }

        var html = node.ToHtmlTreeView();

        return new WebqlQuery(TranslateExpression(node.Children[0].AsInternal()));
    }

    /// <summary>
    /// Translates a CstInternal node into a WebqlExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlExpression object.</returns>
    public static WebqlExpression TranslateExpression(CstInternal node)
    {
        switch (WebqlAstBuilderHelper.GetCstExpressionType(node))
        {
            case WebqlExpressionType.Literal:
                return TranslateLiteralExpression(node);

            case WebqlExpressionType.Reference:
                return TranslateReferenceExpression(node);

            case WebqlExpressionType.ScopeAccess:
                return TranslateScopeAccessExpression(node);

            case WebqlExpressionType.Block:
                return TranslateBlockExpression(node);

            case WebqlExpressionType.Operation:
                return TranslateOperationExpression(node);

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Translates a CstInternal node representing a literal expression into a WebqlLiteralExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlLiteralExpression object.</returns>
    public static WebqlExpression TranslateLiteralExpression(CstInternal node)
    {
        if (node.Children.Length != 1)
        {
            throw new Exception("Invalid literal expression");
        }

        if (node.Children[0] is not CstLeaf leaf)
        {
            throw new Exception("Invalid literal expression");
        }

        switch (leaf.Token.Type)
        {
            case TokenType.Identifier:
                var identifier = leaf.Token.Value.ToString();

                switch (identifier)
                {
                    case "true":
                        return new WebqlLiteralExpression(WebqlLiteralType.Bool, identifier);

                    case "false":
                        return new WebqlLiteralExpression(WebqlLiteralType.Bool, identifier);

                    default:
                        return new WebqlReferenceExpression(leaf.Token.Value.ToString());
                }

            case TokenType.String:
                return new WebqlLiteralExpression(WebqlLiteralType.String, leaf.Token.GetNormalizedStringValue());

            case TokenType.Integer:
                return new WebqlLiteralExpression(WebqlLiteralType.Int, leaf.Token.Value.ToString());

            case TokenType.Float:
                return new WebqlLiteralExpression(WebqlLiteralType.Float, leaf.Token.Value.ToString());

            case TokenType.Hexadecimal:
                return new WebqlLiteralExpression(WebqlLiteralType.Hex, leaf.Token.Value.ToString());

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Translates a CstInternal node representing a reference expression into a WebqlReferenceExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlReferenceExpression object.</returns>
    public static WebqlReferenceExpression TranslateReferenceExpression(CstInternal node)
    {
        if (node.Children.Length != 1)
        {
            throw new Exception("Invalid reference expression");
        }
        if (node.Children[0] is not CstLeaf leaf)
        {
            throw new Exception("Invalid reference expression");
        }

        var identifier = string.Empty;

        if (leaf.Token.Type == TokenType.Identifier)
        {
            identifier = leaf.Token.Value.ToString();
        }
        //* this code adds support to the syntax written in pure JSON format, where keys are written as strings instead of identifiers.
        else if (leaf.Token.Type == TokenType.String)
        {
            identifier = leaf.Token.GetNormalizedStringValue();
        }
        else
        {
            throw new Exception("Invalid scope access expression");
        }

        return new WebqlReferenceExpression(identifier);
    }

    /// <summary>
    /// Translates a CstInternal node representing a scope access expression into a WebqlScopeAccessExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlScopeAccessExpression object.</returns>
    public static WebqlScopeAccessExpression TranslateScopeAccessExpression(CstInternal node)
    {
        if (node.Children.Length != 3)
        {
            throw new Exception("Invalid scope access expression");
        }

        var reference = TranslateReferenceExpression(node.Children[0].AsInternal());
        var expression = TranslateExpression(node.Children[2].AsInternal());

        return new WebqlScopeAccessExpression(reference.Identifier, expression);
    }

    /// <summary>
    /// Translates a CstInternal node representing a block expression into a WebqlBlockExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlBlockExpression object.</returns>
    public static WebqlBlockExpression TranslateBlockExpression(CstInternal node)
    {
        var expressions = new List<WebqlExpression>(node.Children.Length);
        var expressionNodes = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .Select((x, index) => new { Index = index, Node = x })
            .Where(x => x.Index % 2 == 0)
            .Select(x => x.Node);

        foreach (var child in expressionNodes)
        {
            expressions.Add(TranslateExpression(child.AsInternal()));
        }

        return new WebqlBlockExpression(expressions);
    }

    /// <summary>
    /// Translates a CstInternal node representing an operation expression into a WebqlOperationExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlOperationExpression object.</returns>
    public static WebqlOperationExpression TranslateOperationExpression(CstInternal node)
    {
        if (node.Children.Length != 3)
        {
            throw new Exception("Invalid operation expression");
        }

        var @operator = TranslateOperator(node.Children[0].AsInternal());
        var expression = TranslateExpression(node.Children[2].AsInternal());

        return new WebqlOperationExpression(@operator, expression);
    }

    /// <summary>
    /// Translates a CstInternal node representing an operator into a WebqlOperatorType object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlOperatorType object.</returns>
    public static WebqlOperatorType TranslateOperator(CstInternal node)
    {
        if (node.Children.Length != 2)
        {
            throw new Exception("Invalid operator");
        }

        if (node.Children[1] is not CstLeaf leaf)
        {
            throw new Exception("Invalid operator");
        }

        return WebqlAstBuilderHelper.GetCstOperatorType(leaf.Token.ToString());
    }
}
