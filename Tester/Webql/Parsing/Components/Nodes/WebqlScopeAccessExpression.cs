namespace Webql.DocumentSyntax.Parsing.Components;

public class WebqlScopeAccessExpression : WebqlExpression
{
    public override WebqlExpressionType ExpressionType { get; }
    public string Identifier { get; }
    public WebqlExpression Expression { get; }

    public WebqlScopeAccessExpression(string identifier, WebqlExpression expression)
    {
        ExpressionType = WebqlExpressionType.ScopeAccess;
        Identifier = identifier;
        Expression = expression;
    }
}

