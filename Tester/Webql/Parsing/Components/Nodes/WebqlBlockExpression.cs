using Webql.Parsing.Tools;

namespace Webql.Parsing.Components;

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    protected override Dictionary<string, object> Attributes { get; }

    public WebqlBlockExpression(
        IEnumerable<WebqlExpression> expressions, 
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes = null)
    {
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public override WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor)
    {
        return visitor.VisitBlockExpression(this);
    }
}

