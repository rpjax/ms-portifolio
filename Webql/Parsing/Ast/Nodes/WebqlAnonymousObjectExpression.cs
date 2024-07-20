namespace Webql.Parsing.Ast;

public class WebqlAnonymousObjectExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }

    public override WebqlSyntaxNodeMetadata Metadata { get; }

    public override Dictionary<string, object> Attributes { get; }

    public WebqlAnonymousObjectProperty[] Properties { get; }

    public WebqlAnonymousObjectExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        IEnumerable<WebqlAnonymousObjectProperty> properties)
    {
        ExpressionType = WebqlExpressionType.AnonymousObject;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        Properties = properties.ToArray();

        foreach (var property in Properties)
        {
            property.Parent = this;
        }
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        return Properties;
    }

    public override string ToString()
    {
        return $"{{ {string.Join(", ", Properties.Select(x => x.ToString()))} }}";
    }
}
