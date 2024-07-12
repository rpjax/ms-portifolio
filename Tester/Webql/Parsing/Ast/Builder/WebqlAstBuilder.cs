using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Extensions;
using Webql.Core.Analysis;
using Webql.Parsing.Ast.Builder.Extensions;
using Webql.Parsing.Ast.Extensions;

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

        var query = null as WebqlQuery;
        var rootScopeType = WebqlScopeType.Aggregation;
        var metadata = CreateNodeMetadata(node);
        var isEmptyQuery = node.Children.Length == 0;

        var rootContext = new WebqlAstBuildContext(
            scopeType: rootScopeType,
            lhsExpression: CreateSourceReferenceExpression(node)
        );

        node.BindBuildContext(rootContext);

        var expression = isEmptyQuery
            ? null
            : TranslateExpression(node.Children[0].AsInternal());

        return new WebqlQuery(
            metadata: metadata,
            attributes: null,
            expression: expression
        );
    }

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

        var metadata = CreateNodeMetadata(leaf);

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
            metadata: CreateNodeMetadata(leaf),
            attributes: null,
            identifier: identifier
        );
    }

    public static WebqlExpression TranslateScopeAccessExpression(CstInternal node)
    {
        if (node.Children.Length != 3)
        {
            throw new Exception("Invalid scope access expression");
        }

        var referenceNode = node.Children[0].AsInternal();
        var expressionNode = node.Children[2].AsInternal();

        var referenceIdentifier = TranslateReferenceExpressionToIdentifier(referenceNode);
        var referenceNodeMetadata = CreateNodeMetadata(referenceNode);
        
        expressionNode.SetLhsExpressionToMemberAccess(referenceIdentifier);

        var expression = TranslateExpression(expressionNode);
        var expressionIsLiteral = expression.ExpressionType is WebqlExpressionType.Literal;

        if (expressionIsLiteral)
        {
            var lhs = expressionNode.GetLhsExpression();
            var rhs = expression;

            return new WebqlOperationExpression(
                metadata: expression.Metadata,
                attributes: null,
                @operator: WebqlOperatorType.Equals,
                operands: new WebqlExpression[] { lhs, rhs }
            );
        }

        return expression;
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

    public static WebqlExpression TranslateBlockExpression(CstInternal node)
    {
        switch (node.GetScopeType())
        {
            case WebqlScopeType.Aggregation:
                return TranslateAggregationBlockExpression(node);

            case WebqlScopeType.LogicalFiltering:
                return TranslateLogicalBlockExpression(node);

            case WebqlScopeType.Projection:
                return TranslateProjectionBlockExpression(node);
                
            default:
                throw new InvalidOperationException("Invalid block scope type.");
        }
    }
        
    public static WebqlExpression TranslateAggregationBlockExpression(CstInternal node)
    {
        var expressions = new List<WebqlExpression>(node.Children.Length);
        var expressionNodes = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .Select((x, index) => new { Index = index, Node = x })
            .Where(x => x.Index % 2 == 0)
            .Select(x => x.Node)
            .ToArray()
            ;

        if(expressionNodes.Length == 0)
        {
            return new WebqlLiteralExpression(
                metadata: CreateNodeMetadata(node),
                attributes: null,
                literalType: WebqlLiteralType.Int,
                value: "0"
            );
            //return new WebqlBlockExpression(
            //    metadata: CreateNodeMetadata(node),
            //    attributes: null,
            //    expressions: Enumerable.Empty<WebqlExpression>()
            //);
        }

        var lhsExpression = node.GetLhsExpression();

        foreach (var expressionNode in expressionNodes)
        {
            expressionNode.SetLhsExpression(lhsExpression);
            lhsExpression = TranslateExpression(expressionNode.AsInternal());
        }

        return lhsExpression;
    }

    public static WebqlExpression TranslateLogicalBlockExpression(CstInternal node)
    {
        var expressions = new List<WebqlExpression>(node.Children.Length);
        var expressionNodes = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .Select((x, index) => new { Index = index, Node = x })
            .Where(x => x.Index % 2 == 0)
            .Select(x => x.Node)
            .ToArray()
            ;

        if(expressionNodes.Length == 0)
        {
            return new WebqlLiteralExpression(
                metadata: CreateNodeMetadata(node),
                attributes: null,
                literalType: WebqlLiteralType.Bool,
                value: "true"
            );
        }

        foreach (var expressionNode in expressionNodes)
        {
            expressions.Add(TranslateExpression(expressionNode.AsInternal()));
        }

        if (expressions.Count == 1)
        {
            return expressions[0];
        }

        var andExpression = expressions.Aggregate((x, y) => new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: WebqlOperatorType.And,
            operands: new WebqlExpression[] { x, y }
        ));

        return andExpression;
    }

    public static WebqlExpression TranslateProjectionBlockExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

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
            rhsNode.SetLhsExpressionToElementReference();
        }

        var operands = TranslateOperationOperands(node);

        return new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: @operator,
            operands: operands
        );
    }

    public static WebqlExpression[] TranslateOperationOperands(CstInternal node)
    {
        if (node.Children.Length != 3)
        {
            throw new Exception("Invalid operation expression");
        }

        var operatorNode = node.Children[0].AsInternal();
        var rhsNode = node.Children[2].AsInternal();

        var @operator = TranslateOperator(operatorNode);    

        if(@operator == WebqlOperatorType.Aggregate)
        {
            return new WebqlExpression[] { TranslateExpression(rhsNode) };
        }

        var arity = WebqlOperatorAnalyzer.GetOperatorArity(@operator);

        var lhsExpression = node.GetLhsExpression();
        var rhsExpression = TranslateExpression(rhsNode);

        var operands = arity switch
        {
            WebqlOperatorArity.Unary => new WebqlExpression[] { lhsExpression },
            WebqlOperatorArity.Binary => new WebqlExpression[] { lhsExpression, rhsExpression },
            _ => throw new Exception("Unsupported operator arity. Impossible to build the AST with this operator.")
        };

        return operands;
    }

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
     * helper methods.
     */

    public static WebqlExpression CreateSourceReferenceExpression(CstNode node)
    {
        return new WebqlReferenceExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            identifier: WebqlAstSymbols.SourceIdentifier
        );
    }

    public static WebqlExpression CreateElementReferenceExpression(CstNode node)
    {
        return new WebqlReferenceExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            identifier: WebqlAstSymbols.ElementIdentifier
        );
    }

    /// <summary>
    /// Translates a CstNode into a WebqlSyntaxNodeMetadata object.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static WebqlSyntaxNodeMetadata CreateNodeMetadata(CstNode node)
    {
        return new WebqlSyntaxNodeMetadata(
            startPosition: node.Metadata.StartPosition,
            endPosition: node.Metadata.EndPosition
        );
    }

}
