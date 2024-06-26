using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Extensions;

namespace Webql.Parsing.Ast;

/// <summary>
/// Provides methods to translate a CST into a webql AST.
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
            return new WebqlQuery(
                metadata: TranslateNodeMetadata(node),
                attributes: null,
                expression: null
            );
        }

        return new WebqlQuery(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            expression: TranslateExpression(node.Children[0].AsInternal())
        );
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

        var metadata = TranslateNodeMetadata(leaf);

        switch (leaf.Token.Type)
        {
            case TokenType.Identifier:
                var identifier = leaf.Token.Value.ToString();

                switch (identifier)
                {
                    case "null":
                        return new WebqlLiteralExpression(
                            metadata: metadata,
                            attributes: null,
                            literalType: WebqlLiteralType.Null,
                            value: identifier
                        );

                    case "true":
                    case "false":
                        return new WebqlLiteralExpression(
                            metadata: metadata,
                            attributes: null,
                            literalType: WebqlLiteralType.Bool,
                            value: identifier
                        );

                    default:
                        return new WebqlReferenceExpression(
                            metadata: metadata,
                            attributes: null,
                            identifier: identifier
                        );
                }

            case TokenType.String:
                return new WebqlLiteralExpression(
                    metadata: metadata,
                    attributes: null,
                    literalType: WebqlLiteralType.String,
                    value: leaf.Token.GetNormalizedStringValue()
                );

            case TokenType.Integer:
                return new WebqlLiteralExpression(
                    metadata: metadata,
                    attributes: null,
                    literalType: WebqlLiteralType.Int,
                    value: leaf.Token.Value.ToString()
                );

            case TokenType.Float:
                return new WebqlLiteralExpression(
                    metadata: metadata,
                    attributes: null,
                    literalType: WebqlLiteralType.Float,
                    value: leaf.Token.Value.ToString()
                );

            case TokenType.Hexadecimal:
                return new WebqlLiteralExpression(
                    metadata: metadata,
                    attributes: null,
                    literalType: WebqlLiteralType.Hex,
                    value: leaf.Token.Value.ToString()
                );

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

        return new WebqlReferenceExpression(
            metadata: TranslateNodeMetadata(leaf),
            attributes: null,
            identifier: identifier
        );
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

        return new WebqlScopeAccessExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            identifier: reference.Identifier,
            expression: expression
        );
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

        return new WebqlBlockExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            expressions: expressions
        );
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
        var expressionArray = new WebqlExpression[] { expression };

        return new WebqlOperationExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            @operator: @operator,
            operands: expressionArray
        );
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

    /*
     * private helper methods.
     */

    private static WebqlSyntaxNodeMetadata TranslateNodeMetadata(CstNode node)
    {
        return new WebqlSyntaxNodeMetadata(
            startPosition: node.Metadata.StartPosition,
            endPosition: node.Metadata.EndPosition
        );
    }

}
