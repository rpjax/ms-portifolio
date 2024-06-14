using Webql.Parsing.Tools;

namespace Webql.Parsing.Components;

public class WebqlQuery : WebqlSyntaxNode
{
    public override WebqlNodeType NodeType { get; }
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public WebqlExpression? Expression { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlQuery(
        WebqlExpression? expression, 
        WebqlSyntaxNodeMetadata metadata, 
        Dictionary<string, object>? attributes = null)
    {
        NodeType = WebqlNodeType.Query;
        Expression = expression;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public override WebqlSyntaxNode Accept(SyntaxNodeVisitor visitor)
    {
        return visitor.VisitQuery(this);
    }
}

