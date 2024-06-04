namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlReferenceExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    public WebqlReferenceExpression(string identifier)
    {
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
    }
}

