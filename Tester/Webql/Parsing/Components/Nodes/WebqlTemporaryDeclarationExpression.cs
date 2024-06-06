namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }

    public WebqlTemporaryDeclarationExpression(string identifier, string? type, WebqlExpression value, SyntaxNodeMetadata metadata)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
    }
}

