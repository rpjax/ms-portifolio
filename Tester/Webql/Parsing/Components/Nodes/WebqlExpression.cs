namespace Webql.DocumentSyntax.Parsing.Components;

public abstract class WebqlExpression : WebqlAstNode
{
    public override WebqlNodeType NodeType { get; }
    public abstract WebqlExpressionType ExpressionType { get; }

    public WebqlExpression()
    {
        NodeType = WebqlNodeType.Expression;
    }
}

