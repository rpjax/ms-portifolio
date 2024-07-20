namespace Webql.Parsing.Ast;

public class WebqlMemberAccessExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public override Dictionary<string, object> Attributes { get; }
    public WebqlExpression Expression { get; }
    public string MemberName { get; }

    public WebqlMemberAccessExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        WebqlExpression expression,
        string memberName)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.MemberAccess;
        Attributes = attributes ?? new Dictionary<string, object>();
        Expression = expression;
        MemberName = memberName;

        Expression.Parent = this;
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        yield return Expression;
    }

    public override string ToString()
    {
        return $"{Expression}.{MemberName}";
    }
}
