using Microsoft.CodeAnalysis;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Tokenization.Extensions;
using System.Runtime.CompilerServices;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            case CstExpressionType.AnonymousObject:
                return TranslateAnonymousObjectExpression(node);

            default:
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                return TranslateIntLiteral(leaf);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateIntLiteral(CstLeaf leaf)
    {
        return new WebqlLiteralExpression(
            metadata: CreateNodeMetadata(leaf),
            attributes: null,
            literalType: WebqlLiteralType.Int,
            value: leaf.Token.Value.ToString()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateReferenceExpression(CstInternal node)
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

        var lhsExpression = node.GetLhsExpression();

        return new WebqlMemberAccessExpression(
            metadata: CreateNodeMetadata(leaf),
            attributes: null,
            expression: lhsExpression,
            memberName: identifier
        );

        //return new WebqlReferenceExpression(
        //    metadata: CreateNodeMetadata(leaf),
        //    attributes: null,
        //    identifier: identifier
        //);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateBlockExpression(CstInternal node)
    {
        switch (node.GetCstScopeType())
        {
            case WebqlScopeType.LogicalFiltering:
                return TranslateLogicalBlockExpression(node);

            default:
                return TranslateAggregationBlockExpression(node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        if (expressionNodes.Length == 0)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        if (expressionNodes.Length == 0)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateProjectionBlockExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateOperationExpression(CstInternal node)
    {
        var operatorNode = node.Children[1].AsLeaf();
        var @operator = TranslateOperator(operatorNode);
        var operatorScopeType = WebqlAstBuilderHelper.GetOperatorScopeType(@operator, node.GetCstScopeType());

        node.SetCstScopeType(operatorScopeType);

        switch (WebqlOperatorAnalyzer.GetOperatorArity(@operator))
        {
            case WebqlOperatorArity.Unary:
                return TranslateUnaryOperation(node, @operator);

            case WebqlOperatorArity.Binary:
                return TranslateBinaryOperation(node, @operator);

            default:
                throw new Exception("Unsupported operator arity. Impossible to build the AST with this operator.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateUnaryOperation(CstInternal node, WebqlOperatorType @operator)
    {
        /*
        * The AST building is different for those operators, so they are handled separately.
        */
        switch (WebqlOperatorAnalyzer.GetUnaryOperator(@operator))
        {
            case WebqlUnaryOperator.New:
                return TranslateNewExpression(node);
        }

        var operands = TranslateUnaryOperands(node);

        return new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: @operator,
            operands: operands
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression[] TranslateUnaryOperands(CstInternal node)
    {
        if (node.Children.Length != 4)
        {
            throw new Exception("Invalid operation expression");
        }

        var operand = node.Children[3].AsInternal();
        var expression = TranslateExpression(operand);

        return new WebqlExpression[] { expression };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlAnonymousObjectExpression TranslateNewExpression(CstInternal node)
    {
        return TranslateAnonymousObjectExpression(node.Children[2].AsInternal());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateBinaryOperation(CstInternal node, WebqlOperatorType @operator)
    {
        /*
         * The AST building is different for those operators, so they are handled separately.
         */
        switch (WebqlOperatorAnalyzer.GetBinaryOperator(@operator))
        {
            case WebqlBinaryOperator.Or:
                return TranslateOrExpression(node);

            case WebqlBinaryOperator.And:
                return TranslateAndExpression(node);

            case WebqlBinaryOperator.Limit:
                return TranslateLimitExpression(node);

            case WebqlBinaryOperator.Skip:
                return TranslateSkipExpression(node);
        }

        var isCollectionOperator = WebqlOperatorAnalyzer.IsCollectionOperator(@operator);
        var expressionNode = node.Children[3].AsInternal();

        if (isCollectionOperator)
        {
            expressionNode.SetLhsExpressionToElementReference();
        }

        var operands = TranslateBinaryOperands(node);

        return new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: @operator,
            operands: operands
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression[] TranslateBinaryOperands(CstInternal node)
    {
        if (node.Children.Length != 4)
        {
            throw new Exception("Invalid operation expression");
        }

        var operatorNode = node.Children[1].AsLeaf();
        var rhsNode = node.Children[3].AsInternal();

        var @operator = TranslateOperator(operatorNode);

        if (@operator == WebqlOperatorType.Aggregate)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateOrExpression(CstInternal node)
    {
        var expression = null as WebqlExpression;

        for (int i = 4; i < node.Children.Length; i = i + 2)
        {   
            var operandNode = node.Children[i].AsInternal();
            var operand = TranslateExpression(operandNode);

            if(expression is null)
            {
                expression = operand;
            }
            else
            {
                expression = new WebqlOperationExpression(
                    metadata: CreateNodeMetadata(node),
                    attributes: null,
                    @operator: WebqlOperatorType.Or,
                    operands: new WebqlExpression[] { expression, operand }
                );
            }
        }
        
        if(expression is null)
        {
            throw new InvalidOperationException("Invalid OrExpression. The syntax defined by the grammar should only accept non empty array expressions.");
        }

        return expression;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateAndExpression(CstInternal node)
    {
        var expression = null as WebqlExpression;

        for (int i = 4; i < node.Children.Length; i = i + 2)
        {
            var operandNode = node.Children[i].AsInternal();
            var operand = TranslateExpression(operandNode);

            if (expression is null)
            {
                expression = operand;
            }
            else
            {
                expression = new WebqlOperationExpression(
                    metadata: CreateNodeMetadata(node),
                    attributes: null,
                    @operator: WebqlOperatorType.And,
                    operands: new WebqlExpression[] { expression, operand }
                );
            }
        }

        if (expression is null)
        {
            throw new InvalidOperationException("Invalid AndExpression. The syntax defined by the grammar should only accept non empty array expressions.");
        }

        return expression;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateLimitExpression(CstInternal node)
    {
        var rhsNode = node.Children[3].AsLeaf();

        var lhsExpression = node.GetLhsExpression();
        var rhsExpression = TranslateIntLiteral(rhsNode);

        return new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: WebqlOperatorType.Limit,
            operands: new WebqlExpression[] { lhsExpression, rhsExpression }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression TranslateSkipExpression(CstInternal node)
    {
        var rhsNode = node.Children[3].AsLeaf();

        var lhsExpression = node.GetLhsExpression();
        var rhsExpression = TranslateIntLiteral(rhsNode);

        return new WebqlOperationExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            @operator: WebqlOperatorType.Skip,
            operands: new WebqlExpression[] { lhsExpression, rhsExpression }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlOperatorType TranslateOperator(CstLeaf node)
    {
        return WebqlAstBuilderHelper.GetCstOperatorType(node.Token.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlAnonymousObjectExpression TranslateAnonymousObjectExpression(CstInternal node)
    {
        if (node.Children.Length < 2)
        {
            throw new Exception("Invalid anonymous object expression");
        }

        var properties = new List<WebqlAnonymousObjectProperty>(node.Children.Length / 2);

        for (var i = 0; i < node.Children.Length - 1; i += 4)
        {
            var propertyNode = node.Children[i + 1].AsLeaf();
            var valueNode = node.Children[i + 3].AsInternal();

            var property = TranslateAnonymousObjectProperty(propertyNode, valueNode);

            properties.Add(property);
        }

        return new WebqlAnonymousObjectExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            properties: properties
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlAnonymousObjectProperty TranslateAnonymousObjectProperty(CstLeaf propertyNode, CstInternal valueNode)
    {
        var key = propertyNode.Token.Value.ToString();
        var value = TranslateExpression(valueNode);

        return new WebqlAnonymousObjectProperty(
            metadata: CreateNodeMetadata(propertyNode),
            attributes: null,
            name: key,
            value: value
        );
    }

    /*
     * helper methods.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlExpression CreateSourceReferenceExpression(CstNode node)
    {
        return new WebqlReferenceExpression(
            metadata: CreateNodeMetadata(node),
            attributes: null,
            identifier: WebqlAstSymbols.SourceIdentifier
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlSyntaxNodeMetadata CreateNodeMetadata(CstNode node)
    {
        return new WebqlSyntaxNodeMetadata(
            startPosition: node.Metadata.StartPosition,
            endPosition: node.Metadata.EndPosition
        );
    }

}
