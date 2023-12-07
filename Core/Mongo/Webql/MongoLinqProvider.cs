using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Mongo.Webql;

public class MongoLinqProvider : ModularSystem.Webql.Synthesis.LinqProvider
{
    public override Expression TranslateWhereOperator(Context context, NodeTranslator translator, Node node)
    {
        if (!context.IsQueryable())
        {
            throw new Exception("Context must be IQueryable");
        }
        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        var subEntityType = context.GetQueryableType();

        if (subEntityType == null)
        {
            throw new Exception();
        }

        var methodInfo = GetWhereMethodInfo().MakeGenericMethod(subEntityType);

        if (methodInfo == null)
        {
            throw new InvalidOperationException();
        }

        var subExpressionParameter = Expression.Parameter(subEntityType, "x");
        var subContext = new Context(subEntityType, subExpressionParameter, context);
        var subExpressionBody = translator.Translate(subContext, node);
        var subExpression = Expression.Lambda(subExpressionBody, subExpressionParameter);

        var methodArgs = new Expression[] { context.InputExpression, subExpression };

        return Expression.Call(null, methodInfo, methodArgs);
    }

    private static MethodInfo GetWhereMethodInfo()
    {
        return typeof(MongoQueryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
    }

}
