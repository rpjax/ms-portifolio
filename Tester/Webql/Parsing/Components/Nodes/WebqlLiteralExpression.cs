namespace Webql.DocumentSyntax.Parsing.Components;

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

