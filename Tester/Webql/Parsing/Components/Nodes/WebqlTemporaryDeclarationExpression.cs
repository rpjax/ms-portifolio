namespace Webql.DocumentSyntax.Parsing.Components;

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

