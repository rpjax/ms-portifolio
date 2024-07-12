
namespace Webql.Parsing.Ast;

public class WebqlTypeConversionExpression : WebqlExpression
{
    public override WebqlSyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public override Dictionary<string, object> Attributes { get; }
    public WebqlExpression Expression { get; }
    public Type TargetType { get; }

    public WebqlTypeConversionExpression(
        WebqlSyntaxNodeMetadata metadata,
        Dictionary<string, object>? attributes,
        WebqlExpression expression, 
        Type targetType)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.TypeConversion;
        Attributes = attributes ?? new Dictionary<string, object>();
        Expression = expression;
        TargetType = targetType;

        Expression.Parent = this;
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        yield return Expression;
    }

    public override string ToString()
    {
        return $"({TargetType}) {Expression}";
    }
}
