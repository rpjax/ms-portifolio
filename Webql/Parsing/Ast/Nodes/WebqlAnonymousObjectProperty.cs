using Webql.Parsing.Analysis;

namespace Webql.Parsing.Ast;

public class WebqlAnonymousObjectProperty : WebqlSyntaxNode
{
    public override WebqlNodeType NodeType { get; }

    public override WebqlSyntaxNodeMetadata Metadata { get; }

    public override Dictionary<string, object> Attributes { get; }

    public string Name { get; }

    public WebqlExpression Value { get; }

    public WebqlAnonymousObjectProperty(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        string name,
        WebqlExpression value)
    {
        NodeType = WebqlNodeType.AnonymousObjectProperty;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        Name = name;
        Value = value;

        Value.Parent = this;
    }

    public override WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor)
    {
        return visitor.VisitAnonymousObjectProperty(this);
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}
