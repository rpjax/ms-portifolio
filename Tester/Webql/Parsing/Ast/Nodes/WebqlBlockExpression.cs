namespace Webql.Parsing.Ast;

public enum WebqlScopeType
{
    Aggregation,
    LogicalFiltering,
    Projection,
}

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override Dictionary<string, object> Attributes { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlScopeType ScopeType { get; set; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        WebqlScopeType scopeType,
        IEnumerable<WebqlExpression> expressions) 
    {
        Metadata = metadata;
        Attributes = attributes ?? new Dictionary<string, object>();
        ExpressionType = WebqlExpressionType.Block;
        ScopeType = scopeType;
        Expressions = expressions.ToArray();

        foreach (var expression in Expressions)
        {
            expression.Parent = this;
        }
    }

    public override string ToString()
    {
        return $"{{ {string.Join(", ", Expressions.Select(x => x.ToString()))} }}";
    }

}
