using Webql.Parsing.Ast.Extensions;

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

        foreach (var expression in Expressions)
        {
            expression.Parent = this;
        }
    }

    public override IEnumerable<WebqlSyntaxNode> GetChildren()
    {
        return Expressions;
    }

    public override string ToString()
    {
        return $"{{ {string.Join(", ", Expressions.Select(x => x.ToString()))} }}";
    }

}
