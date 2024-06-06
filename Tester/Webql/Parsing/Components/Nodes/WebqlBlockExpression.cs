namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlBlockExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(IEnumerable<WebqlExpression> expressions, SyntaxNodeMetadata metadata)
    {
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
        Metadata = metadata;
    }
}

