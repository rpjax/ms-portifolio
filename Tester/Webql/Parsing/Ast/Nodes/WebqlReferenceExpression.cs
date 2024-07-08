namespace Webql.Parsing.Ast;

public class WebqlReferenceExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    public WebqlReferenceExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        string identifier)
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        return Enumerable.Empty<WebqlSyntaxNode>();
    }

    public override string ToString()
    {
        return Identifier;
    }

}
