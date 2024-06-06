namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlLiteralExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlLiteralType LiteralType { get; }
    public string Value { get; }

    public WebqlLiteralExpression(WebqlLiteralType literalType, string value, SyntaxNodeMetadata metadata)
    {
        ExpressionType = WebqlExpressionType.Literal;
        LiteralType = literalType;
        Value = value;
        Metadata = metadata;
    }
}

