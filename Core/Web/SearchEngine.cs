using ModularSystem.Core;
using ModularSystem.Web.Expressions;
using System.Linq.Expressions;

namespace ModularSystem.Web;

public static class SearchEngine
{
    public static ExpressionSerializer ExpressionSerializer { get; set; } = DefaultExpressionSerializer();

    static ExpressionSerializer DefaultExpressionSerializer()
    {
        return new ExpressionSerializer(new ExpressionSerializer.Configs()
        {
            TypeSerializerOptions = new TypeSerializer.Options()
            {
                UseAssemblyName = true,
            }
        });
    }
}

public class FilterExpression
{
    public ExpressionNode? Node { get; set; }

    public Expression<Func<T, bool>>? ToExpression<T>() where T : class
    {
        if (Node == null)
        {
            return null;
        }

        var error = SearchEngine.ExpressionSerializer.LambdaEvaluate<Func<T, bool>>(Node, out var lambdaExpression);

        if (error == null)
        {
            return lambdaExpression!;
        }

        throw new AppException("Could not evaluate ExpressionNode to filter 'Expression<Func<T, bool>>'.", ExceptionCode.InvalidInput, error, Node);
    }
}

public class SerializedSearch<T> where T : class
{
    public FilterExpression Filter { get; set; } = new FilterExpression();

    public Query<T> ToSearch()
    {
        return new Query<T>()
        {
            Filter = Filter.ToExpression<T>()
        };
    }
}


//public class FilterExpressionBuilder<T> where T : class
//{
//    Type type;
//    ExpressionNode? node;

//    public FilterExpressionBuilder(ExpressionNode? node = null)
//    {
//        type = typeof(T);
//        this.node = node;
//    }

//    public System.Linq.Expressions.Expression<Func<T, bool>> Build()
//    {
//        ParameterExpression parameterExpression = Expression.Parameter(type);
//        Expression expression = Expression.Empty();

//        return EmptyExpression<T>();
//        return Expression.Lambda<Func<T, bool>>(expression, new[] { parameterExpression });
//    }

//    System.Linq.Expressions.Expression<Func<T, bool>> EmptyExpression<T>() where T : class
//    {
//        return x => true;
//    }
//}