namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlReferenceExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }

    public WebqlReferenceExpression(string identifier, SyntaxNodeMetadata metadata)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.Reference;
        Identifier = identifier;
    }
}

