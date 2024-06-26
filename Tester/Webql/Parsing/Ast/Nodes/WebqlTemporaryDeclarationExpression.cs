namespace Webql.Parsing.Ast;

public class WebqlTemporaryDeclarationExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public string? Type { get; }
    public WebqlExpression Value { get; }

    public WebqlTemporaryDeclarationExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        string identifier, 
        string? type, 
        WebqlExpression value)
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.TemporaryDeclaration;
        Identifier = identifier;
        Type = type;
        Value = value;
    }

}
