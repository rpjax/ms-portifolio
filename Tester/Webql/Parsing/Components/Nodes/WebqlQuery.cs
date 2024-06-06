namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlQuery : WebqlAstNode
{
    public override WebqlNodeType NodeType { get; }
    public override SyntaxNodeMetadata Metadata { get; }
    public WebqlExpression? Expression { get; }

    public WebqlQuery(WebqlExpression? expression, SyntaxNodeMetadata metadata)
    {
        NodeType = WebqlNodeType.Query;
        Expression = expression;
        Metadata = metadata;
    }
}

