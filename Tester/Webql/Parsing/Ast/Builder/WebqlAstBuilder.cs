using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Extensions;
using Webql.Core.Analysis;
using Webql.Parsing.Ast.Builder.Extensions;

namespace Webql.Parsing.Ast.Builder;

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

        var accumulatorReference = new WebqlReferenceExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            identifier: AstHelper.AccumulatorIdentifier
        );

        node.SetAccumulatorExpression(accumulatorReference);
        node.SetScopeType(WebqlScopeType.Aggregation);

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
            case CstExpressionType.Literal:
                return TranslateLiteralExpression(node);

            case CstExpressionType.Reference:
                return TranslateReferenceExpression(node);

            case CstExpressionType.ScopeAccess:
                return TranslateScopeAccessExpression(node);

            case CstExpressionType.Block:
                return TranslateBlockExpression(node);

            case CstExpressionType.Operation:
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
                    value: leaf.Token.Value.ToString()
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

    public static string TranslateReferenceExpressionToIdentifier(CstInternal node)
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

        return identifier;
    }

    /// <summary>
    /// Translates a CstInternal node representing a scope access expression into a WebqlScopeAccessExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlScopeAccessExpression object.</returns>
    public static WebqlExpression TranslateScopeAccessExpression(CstInternal node)
    {
        if (node.Children.Length != 3)
        {
            throw new Exception("Invalid scope access expression");
        }

        var referenceNode = node.Children[0].AsInternal();
        var expressionNode = node.Children[2].AsInternal();

        var referenceIdentifier = TranslateReferenceExpressionToIdentifier(referenceNode);
        var referenceNodeMetadata = TranslateNodeMetadata(referenceNode);

        var accumulatorExpression = node.GetAccumulatorExpression();
        
        var memberAccess = new WebqlMemberAccessExpression(
            metadata: referenceNodeMetadata,
            attributes: null,
            expression: accumulatorExpression,
            memberName: referenceIdentifier
        );

        expressionNode.SetAccumulatorExpression(memberAccess);

        var expression = TranslateExpression(expressionNode);

        if (expression.ExpressionType is WebqlExpressionType.Literal)
        {
            return new WebqlOperationExpression(
                metadata: expression.Metadata,
                attributes: null,
                @operator: WebqlOperatorType.Equals,
                operands: new WebqlExpression[] { memberAccess, expression }
            );
        }

        return expression;
    }

    /// <summary>
    /// Translates a CstInternal node representing a block expression into a WebqlBlockExpression object.
    /// </summary>
    /// <param name="node">The CstInternal node to translate.</param>
    /// <returns>The translated WebqlBlockExpression object.</returns>
    public static WebqlExpression TranslateBlockExpression(CstInternal node)
    {
        var expressions = new List<WebqlExpression>(node.Children.Length);
        var expressionNodes = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .Select((x, index) => new { Index = index, Node = x })
            .Where(x => x.Index % 2 == 0)
            .Select(x => x.Node);

        var isAggregation = node.GetScopeType() == WebqlScopeType.Aggregation;
        
        if(isAggregation)
        {
            var accumulatorReference = new WebqlReferenceExpression(
                metadata: TranslateNodeMetadata(node),
                attributes: null,
                identifier: AstHelper.AccumulatorIdentifier
            );

            expressionNodes
                .Skip(1)
                .ToList()
                .ForEach(x => x.SetAccumulatorExpression(accumulatorReference));
        }

        foreach (var child in expressionNodes)
        {
            expressions.Add(TranslateExpression(child.AsInternal()));
        }

        if(expressions.Count == 1 && !node.IsBlockSimplificationDisabled())
        {
            return expressions[0];
        }

        return new WebqlBlockExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            scopeType: node.GetScopeType(),
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

        var operatorNode = node.Children[0].AsInternal();
        var rhsNode = node.Children[2].AsInternal();

        var @operator = TranslateOperator(operatorNode);    
        var isCollectionOperator = WebqlOperatorAnalyzer.IsCollectionOperator(@operator);
        var operatorScopeType = WebqlAstBuilderHelper.GetOperatorScopeType(@operator, node.GetScopeType());

        rhsNode.SetScopeType(operatorScopeType);

        if (isCollectionOperator)
        {
            var accumulatorReference = new WebqlReferenceExpression(
                metadata: TranslateNodeMetadata(node),
                attributes: null,
                identifier: AstHelper.AccumulatorIdentifier
            );

            rhsNode.SetAccumulatorExpression(accumulatorReference);
        }

        var lhsExpression = node.GetAccumulatorExpression();
        var rhsExpression = TranslateExpression(rhsNode);

        var operands = @operator is WebqlOperatorType.Aggregate 
            ? new WebqlExpression[] { rhsExpression }
            : new WebqlExpression[] { lhsExpression, rhsExpression };

        return new WebqlOperationExpression(
            metadata: TranslateNodeMetadata(node),
            attributes: null,
            @operator: @operator,
            operands: operands
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
