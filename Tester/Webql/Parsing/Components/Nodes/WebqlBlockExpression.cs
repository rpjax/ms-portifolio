namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlBlockExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlExpression[] Expressions { get; }

    public WebqlBlockExpression(IEnumerable<WebqlExpression> expressions)
    {
        ExpressionType = WebqlExpressionType.Block;
        Expressions = expressions.ToArray();
    }
}

