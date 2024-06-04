namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlQuery : WebqlAstNode
{
    public override WebqlNodeType NodeType { get; }
    public WebqlExpression? Expression { get; }

    public WebqlQuery(WebqlExpression? expression)
    {
        NodeType = WebqlNodeType.Query;
        Expression = expression;
    }
}

