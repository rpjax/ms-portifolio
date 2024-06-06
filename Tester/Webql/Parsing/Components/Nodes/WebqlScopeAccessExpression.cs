namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlScopeAccessExpression : WebqlExpression
{
    public override SyntaxNodeMetadata Metadata { get; }
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public WebqlExpression Expression { get; }

    public WebqlScopeAccessExpression(string identifier, WebqlExpression expression, SyntaxNodeMetadata metadata)
    {
        Metadata = metadata;
        ExpressionType = WebqlExpressionType.ScopeAccess;
        Identifier = identifier;
        Expression = expression;
    }
}

