namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlOperationExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public WebqlOperatorType Operator { get; }
    public WebqlExpression[] Operands { get; }

    public WebqlOperationExpression(WebqlOperatorType @operator, WebqlExpression[] operands, SyntaxNodeMetadata metadata)
    {
        ExpressionType = WebqlExpressionType.Operation;
        Operator = @operator;
        Operands = operands;
        Metadata = metadata;
    }

    public WebqlOperationExpression(
        WebqlOperatorType @operator, 
        IEnumerable<WebqlExpression> operands, 
        SyntaxNodeMetadata metadata) 
        : this(@operator, operands.ToArray(), metadata)
    {

    }
}

