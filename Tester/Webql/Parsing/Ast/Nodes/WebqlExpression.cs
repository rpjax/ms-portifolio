using Webql.Parsing.Analysis;

namespace Webql.Parsing.Ast;

public abstract class WebqlExpression : WebqlSyntaxNode
{
    public override WebqlNodeType NodeType { get; }
    public abstract WebqlExpressionType ExpressionType { get; }

    public WebqlExpression()
    {
        NodeType = WebqlNodeType.Expression;
    }

    public override WebqlSyntaxNode Accept(SyntaxTreeVisitor visitor)
    {
        return visitor.VisitExpression(this);
    }
}

