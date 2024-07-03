namespace Webql.Parsing.Ast.Builder;

public class AstContext
{
    private Stack<WebqlExpression> ExpressionStack { get; }

    public AstContext()
    {
        ExpressionStack = new Stack<WebqlExpression>();
    }

    public WebqlExpression GetLhsExpression()
    {
        if(ExpressionStack.Count == 0)
        {
            throw new InvalidOperationException("No LHS expression found");
        }

        return ExpressionStack.Peek();
    }

    public void PushLhsExpression(WebqlExpression expression)
    {
        ExpressionStack.Push(expression);
    }

    public void PopLhsExpression()
    {
        if(ExpressionStack.Count == 0)
        {
            throw new InvalidOperationException("No LHS expression found");
        }

        ExpressionStack.Pop();
    }
}
