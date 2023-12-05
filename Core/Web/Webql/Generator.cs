using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

public class GeneratorOptions
{
    public MethodInfo WhereProvider { get; set; } 
    public MethodInfo ProjectProvider { get; set; }

    public GeneratorOptions()
    {
        WhereProvider = DefaultWhere();
        ProjectProvider = DefaultSelect();
    }

    private static MethodInfo DefaultWhere()
    {
        var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 1
                            && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0])
                            && x.Params[1].ParameterType == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(x.Args[0], typeof(bool))))
                .Select(x => x.Method)
                .Single();

        return method;
    }

    private static MethodInfo DefaultSelect()
    {
        var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 2
                            && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0])
                            && x.Params[1].ParameterType == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(x.Args[0], x.Args[1])))
                .Select(x => x.Method)
                .Single();

        return method;
    }
}

public class Generator
{
    private GeneratorOptions Options { get; }
    private NodeParser NodeParser { get; }

    public Generator(GeneratorOptions? options = null)
    {
        Options = options ?? new();
        NodeParser = new(Options);
    }

    public Expression CreateExpression(Node node, Type type, ParameterExpression? parameter = null)
    {
        var queryableType = typeof(IEnumerable<>).MakeGenericType(type);
        parameter ??= Expression.Parameter(queryableType, "root");
        var context = new Context(queryableType, parameter);

        return NodeParser.Parse(context, node);
    }

    public TranslatedQueryable CreateQueryable(Node node, Type type, IQueryable queryable)
    {
        var inputType = typeof(IQueryable<>).MakeGenericType(type);
        var parameter = Expression.Parameter(inputType, "root");
        var context = new Context(inputType, parameter);
        var expression = NodeParser.Parse(context, node);

        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = typeof(IQueryable<>).MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(inputType, outputType);

        var lambdaExpression = Expression.Lambda(lambdaExpressionType, expression, parameter);
        var lambda = lambdaExpression.Compile();

        var transformedQueryable = lambda.DynamicInvoke(queryable);

        if (transformedQueryable == null)
        {
            throw new Exception();
        }

        return new TranslatedQueryable(inputType.GenericTypeArguments.First(), outputType.GenericTypeArguments.Last(), transformedQueryable);
    }

    public TranslatedQueryable CreateQueryable<T>(Node node, IQueryable<T> queryable)
    {
        return CreateQueryable(node, typeof(T), queryable);
    }

}
