using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.Parsing.Components;

public abstract class WebqlExpression : WebqlSyntaxNode
{
    public override WebqlNodeType NodeType { get; }
    public abstract WebqlExpressionType ExpressionType { get; }

    public WebqlExpression()
    {
        NodeType = WebqlNodeType.Expression;
    }
}

