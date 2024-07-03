using Webql.Parsing.Analysis;

namespace Webql.Parsing.Ast;

public class WebqlQuery : WebqlSyntaxNode
{
    public override WebqlNodeType NodeType { get; }
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public WebqlExpression? Expression { get; }

    public WebqlQuery(
        WebqlSyntaxNodeMetadata metadata, 
        Dictionary<string, object>? attributes,
        WebqlExpression? expression)
    {
        NodeType = WebqlNodeType.Query;
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        Expression = expression;

        if (Expression != null)
        {
            Expression.Parent = this;
        }
    }

    public override string ToString()
    {
        return Expression?.ToString() ?? string.Empty;
    }

    public override WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor)
    {
        return visitor.VisitQuery(this);
    }
}
