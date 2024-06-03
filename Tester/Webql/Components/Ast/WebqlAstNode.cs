using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using Webql.DocumentSyntax.Parsing;

namespace Webql.Components.Ast;

public enum WebqlNodeType
{
    Query,
    Expression,
}

public abstract class WebqlAstNode
{
    public abstract WebqlNodeType NodeType { get; }
}

public class WebqlQuery : WebqlAstNode
{
    public override WebqlNodeType NodeType { get; }
    public WebqlExpression? Expression { get; }

    public WebqlQuery(WebqlExpression? expression)
    {
        NodeType = WebqlNodeType.Query;
        Expression = expression;
    }
}

public enum WebqlExpressionType
{
    Literal,
    Reference,
    TemporaryDeclaration,
    Block,
    Operation
}

public abstract class WebqlExpression : WebqlAstNode
{
    public override WebqlNodeType NodeType { get; }
    public abstract WebqlExpressionType ExpressionType { get; }

    public WebqlExpression()
    {
        NodeType = WebqlNodeType.Expression;
    }
}

public enum WebqlLiteralType
{
    Bool,
    Null,
    Int,
    Float,
    Hex,
    String
}

public class WebqlLiteralExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlLiteralType LiteralType { get; }
    public string Value { get; }

    public WebqlLiteralExpression(WebqlLiteralType literalType, string value)
    {
        ExpressionType = WebqlExpressionType.Literal;
        LiteralType = literalType;
        Value = value;
    }
}

public class WebqlReferenceExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    public WebqlReferenceExpression(string identifier)
    {
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
    }
}

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }

    public WebqlTemporaryDeclarationExpression(string identifier, string? type, WebqlExpression value)
    {
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
    }
}

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(IEnumerable<WebqlExpression> expressions)
    {
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
    }
}

public enum WebqlOperatorType
{
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEquals,
    GreaterThan,
    GreaterThanOrEquals,
    And,
    Or,
    Not
}

public class WebqlOperationExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    public WebqlOperationExpression(WebqlOperatorType @operator, IEnumerable<WebqlExpression> operands)
    {
        ExpressionType = WebqlExpressionType.Operation;
        Operator = @operator;
        Operands = operands.ToArray();
    }
}

public class WebqlAstBuilder
{
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

        return new WebqlQuery(TranslateExpression(node.Children[0].AsInternal()));
    }

    public static WebqlExpression TranslateExpression(CstInternal node)
    {
        switch (WebqlAstBuilderHelper.GetCstExpressionType(node))
        {
            case WebqlCstExpressionType.Literal:
                return TranslateLiteralExpression(node);

            case WebqlCstExpressionType.Reference:
                return TranslateReferenceExpression(node);

            case WebqlCstExpressionType.ScopeAccess:
                return TranslateScopeAccessExpression(node);

            case WebqlCstExpressionType.Block:
                return TranslateBlockExpression(node);

            case WebqlCstExpressionType.Operation:
                return TranslateOperationExpression(node);

            default:
                throw new InvalidOperationException();
        }
    }

    public static WebqlExpression TranslateLiteralExpression(CstInternal node)
    {
        if(node.Children.Length != 1)
        {
            throw new Exception("Invalid literal expression");
        }

        if (node.Children[0] is not CstLeaf leaf)
        {
            throw new Exception("Invalid literal expression");
        }

        switch (leaf.Token.Type)
        {
            case ModularSystem.Core.TextAnalysis.Tokenization.TokenType.Identifier:
                break;

            case ModularSystem.Core.TextAnalysis.Tokenization.TokenType.String:
                break;

            case ModularSystem.Core.TextAnalysis.Tokenization.TokenType.Integer:
                break;

            case ModularSystem.Core.TextAnalysis.Tokenization.TokenType.Float:
                break;

            case ModularSystem.Core.TextAnalysis.Tokenization.TokenType.Hexadecimal:
                break;

            default:
                throw new InvalidOperationException();
        }

        throw new NotImplementedException();   
    }

    public static WebqlExpression TranslateReferenceExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

    public static WebqlExpression TranslateScopeAccessExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

    public static WebqlExpression TranslateBlockExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

    public static WebqlExpression TranslateOperationExpression(CstInternal node)
    {
        throw new NotImplementedException();
    }

}

public enum WebqlCstExpressionType
{
    Literal,
    Reference,
    ScopeAccess,
    Block,
    Operation
}

public static class WebqlAstBuilderHelper
{
    public static WebqlCstExpressionType GetCstExpressionType(CstInternal node)
    {
        switch (node.Name)
        {
            case "literal_expression":
                return WebqlCstExpressionType.Literal;

            case "reference_expression":
                return WebqlCstExpressionType.Reference;

            case "scope_access_expression":
                return WebqlCstExpressionType.ScopeAccess;

            case "block_expression":
                return WebqlCstExpressionType.Block;

            case "operation_expression":
                return WebqlCstExpressionType.Operation;

            default:
                throw new InvalidOperationException();
        }
    }
}