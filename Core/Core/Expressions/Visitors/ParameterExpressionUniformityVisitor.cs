using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

/// <summary>
/// A helper class designed to ensure that all parameter expressions within filter expressions refer to the same root parameter. <br/>
/// This is essential for combining multiple expressions seamlessly.
/// </summary>
public class ParameterExpressionUniformityVisitor : ExpressionVisitor
{
    /// <summary>
    /// Gets the root parameter expression identified during the visitation process.
    /// </summary>
    private ParameterExpression? RootParameterExpression { get; set; } = null;

    /// <summary>
    /// This is a helper method that creates an instance of <see cref="ParameterExpressionUniformityVisitor"/> and call the visit method.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression? Run(Expression? expression)
    {
        return new ParameterExpressionUniformityVisitor().Visit(expression);
    }

    /// <summary>
    /// Visits and potentially modifies a lambda expression.
    /// </summary>
    /// <typeparam name="TDelegate">The type of delegate of the lambda expression.</typeparam>
    /// <param name="node">The lambda expression to visit.</param>
    /// <returns>The modified lambda expression.</returns>
    protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
    {
        if (node.NodeType != ExpressionType.Lambda)
        {
            return base.VisitLambda(node);
        }

        var lambdaExpression = node.TypeCast<LambdaExpression>();

        RootParameterExpression = lambdaExpression.Parameters.First();

        return base.VisitLambda(node);
    }

    /// <summary>
    /// Visits and potentially modifies a member access expression.
    /// </summary>
    /// <param name="node">The member access expression to visit.</param>
    /// <returns>The modified member access expression.</returns>
    protected override Expression VisitMember(MemberExpression node)
    {
        var helper = new Helper();
        var isLhs = helper.IsLhs(node);

        if (node.Expression?.Type == RootParameterExpression?.Type && isLhs)
        {
            return Expression.MakeMemberAccess(RootParameterExpression, node.Member);
        }

        return base.VisitMember(node);
    }

    private class Helper : ExpressionVisitor
    {
        bool isLhs = true;

        public bool IsLhs(Expression expression)
        {
            Visit(expression); 
            return isLhs;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            isLhs = false;
            return base.VisitConstant(node);
        }
    }
}
