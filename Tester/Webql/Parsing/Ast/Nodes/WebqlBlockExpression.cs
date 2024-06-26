namespace Webql.Parsing.Ast;

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        IEnumerable<WebqlExpression> expressions) 
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
    }

}
