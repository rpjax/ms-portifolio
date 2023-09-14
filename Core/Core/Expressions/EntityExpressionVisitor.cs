using System;
using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

public class EntityExpressionVisitor<T> : ExpressionVisitor, IVisitor<Expression> where T : IQueryableModel
{
    public Func<ParameterExpression, Expression>? CreateIdSelectorFunction { get; set; }
    public Func<string, object>? ParseIdFunction { get; set; }

    private ParameterExpression? RootParameter { get; set; }

    public void Reset()
    {
        RootParameter = null;
    }

    protected override Expression VisitLambda<TSignature>(Expression<TSignature> node)
    {
        if (RootParameter != null || node.NodeType != ExpressionType.Lambda)
        {
            return base.VisitLambda(node);
        }

        var lambdaExp = node.TypeCast<LambdaExpression>();

        if (lambdaExp.Parameters.IsEmpty())
        {
            return base.VisitLambda(node);
        }

        RootParameter = lambdaExp.Parameters.First();
        return base.VisitLambda(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var translateIdEquals =
            node.Method.DeclaringType == typeof(EntityLinq) &&
            node.Method.Name == nameof(EntityLinq.IdEqualsFlag) &&
            CreateIdSelectorFunction != null &&
            ParseIdFunction != null;

        if (translateIdEquals)
        {
            if (RootParameter == null)
            {
                throw new InvalidOperationException();
            }

            Expression left = CreateIdSelectorFunction.Invoke(RootParameter);
            Expression right = Expression.Empty();

            if (node.Arguments.IsEmpty())
            {
                throw new InvalidOperationException();
            }

            var firstArgument = node.Arguments.First();

            if (firstArgument.NodeType == ExpressionType.Constant)
            {
                var constExp = firstArgument.TypeCast<ConstantExpression>();
                var value = constExp.Value;

                if (value is not string)
                {
                    throw new InvalidOperationException();
                }

                var strValue = value.TypeCast<string>();
                var parsedValue = ParseIdFunction(strValue);
                right = Expression.Constant(parsedValue, parsedValue.GetType());
            }
            else if (firstArgument.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = firstArgument.TypeCast<MemberExpression>();

                if (memberExp.Expression is not ConstantExpression)
                {
                    throw new InvalidOperationException();
                }

                var constExp = memberExp.Expression.TypeCast<ConstantExpression>();
                var field = memberExp.Expression.Type.GetField(memberExp.Member.Name);
                var value = field.GetValue(constExp.Value);

                if (value is not string)
                {
                    throw new InvalidOperationException();
                }

                var strValue = value.TypeCast<string>();
                var parsedValue = ParseIdFunction(strValue);
                right = Expression.Constant(parsedValue, parsedValue.GetType());
            }
            else
            {
                throw new InvalidOperationException();
            }

            return Expression.Equal(left, right);
        }

        return base.VisitMethodCall(node);
    }
}
