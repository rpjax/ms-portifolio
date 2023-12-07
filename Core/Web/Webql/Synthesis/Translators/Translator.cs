using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

public class TranslatorOptions
{
    public LinqProvider LinqProvider { get; set; } = new LinqProvider();

    public ArithmeticOperatorsTranslator ArithmeticOperatorsTranslator { get; }
    public RelationalOperatorsTranslator RelationalOperatorsParser { get; }
    public LogicalOperatorsTranslator LogicalOperatorsParser { get; }
    public SemanticOperatorsTranslator SemanticOperatorsParser { get; }
    public QueryableOperatorsTranslator QueryableOperatorsParser { get; }

    public Type QueryableType => LinqProvider.GetQueryableType();

    public MethodInfo WhereProvider { get; set; }
    public MethodInfo ProjectProvider { get; set; }
    public MethodInfo TakeProvider { get; set; }
    public MethodInfo SkipProvider { get; set; }
    public MethodInfo CountProvider { get; set; }
    public MethodInfo MinProvider { get; set; }
    public MethodInfo MaxProvider { get; set; }

    public MethodInfo IntSumProvider { get; set; }
    public MethodInfo Int64SumProvider { get; set; }
    public MethodInfo FloatSumProvider { get; set; }
    public MethodInfo DoubleSumProvider { get; set; }
    public MethodInfo DecimalSumProvider { get; set; }

    public MethodInfo IntAverageProvider { get; set; }
    public MethodInfo Int64AverageProvider { get; set; }
    public MethodInfo FloatAverageProvider { get; set; }
    public MethodInfo DoubleAverageProvider { get; set; }
    public MethodInfo DecimalAverageProvider { get; set; }

    public bool TakeSupportsInt64 { get; set; } = false;
    public bool SkipSupportsInt64 { get; set; } = false;

    public TranslatorOptions()
    {
        WhereProvider = DefaultWhere();
        ProjectProvider = DefaultSelect();
        TakeProvider = DefaultTake();
        SkipProvider = DefaultSkip();
        CountProvider = DefaultCount();
        MinProvider = DefaultMin();
        MaxProvider = DefaultMax();

        IntSumProvider = DefaultIntSum();
        Int64SumProvider = DefaultInt64Sum();
        FloatSumProvider = DefaultFloatSum();
        DoubleSumProvider = DefaultDoubleSum();
        DecimalSumProvider = DefaultDecimalSum();

        IntAverageProvider = DefaultIntAverage();
        Int64AverageProvider = DefaultInt64Average();
        FloatAverageProvider = DefaultFloatAverage();
        DoubleAverageProvider = DefaultDoubleAverage();
        DecimalAverageProvider = DefaultDecimalAverage();
    }

    private static MethodInfo DefaultWhere()
    {
        return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

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

    //private static MethodInfo DefaultTake()
    //{
    //    var method = typeof(Queryable).GetMethods()
    //            .Where(m => m.Name == "Take" && m.IsGenericMethodDefinition)
    //            .Select(m => new
    //            {
    //                Method = m,
    //                Params = m.GetParameters(),
    //                Args = m.GetGenericArguments()
    //            })
    //            .Where(x => x.Params.Length == 2
    //                        && x.Args.Length == 1  // 'Take' has only one generic argument
    //                        && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0])
    //                        && x.Params[1].ParameterType == typeof(int))  // Second parameter is an int
    //            .Select(x => x.Method)
    //            .Single();

    //    return method;
    //}

    private static MethodInfo DefaultTake()
    {
        return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Take" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                        m.GetParameters()[1].ParameterType == typeof(int));
    }

    private static MethodInfo DefaultSkip()
    {
        var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Skip" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 1  // 'Take' has only one generic argument
                            && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0])
                            && x.Params[1].ParameterType == typeof(int))  // Second parameter is an int
                .Select(x => x.Method)
                .Single();

        return method;
    }

    private static MethodInfo DefaultCount()
    {
        // Find the 'Count' method in the Queryable class
        var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Count" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 1
                            && x.Args.Length == 1  // 'Count' has only one generic argument
                            && x.Params[0].ParameterType == typeof(IQueryable<>).MakeGenericType(x.Args[0]))
                .Select(x => x.Method)
                .Single();

        return method;
    }

    private static MethodInfo DefaultMin()
    {
        var method = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "Min" && m.IsGenericMethodDefinition)
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

    private static MethodInfo DefaultMax()
    {
        var method = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "Max" && m.IsGenericMethodDefinition)
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

    private static MethodInfo FindSumMethod(Type returnType)
    {
        return typeof(Queryable).GetMethods()
            .Where(m => m.Name == "Sum" && m.IsGenericMethodDefinition)
            .SelectMany(m => m.GetParameters()
                .Select(p => new { Method = m, ParamType = p.ParameterType }))
            .Where(x => x.ParamType.IsGenericType &&
                        x.ParamType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                        x.ParamType.GetGenericArguments()[0].IsGenericType &&
                        x.ParamType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                        x.ParamType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType)
            .Select(x => x.Method)
            .First();
    }

    private static MethodInfo DefaultIntSum()
    {
        return FindSumMethod(typeof(int));
    }

    private static MethodInfo DefaultInt64Sum()
    {
        return FindSumMethod(typeof(long));
    }

    private static MethodInfo DefaultFloatSum()
    {
        return FindSumMethod(typeof(float));
    }

    private static MethodInfo DefaultDoubleSum()
    {
        return FindSumMethod(typeof(double));
    }

    private static MethodInfo DefaultDecimalSum()
    {
        return FindSumMethod(typeof(decimal));
    }

    private static MethodInfo FindAverageMethod(Type returnType)
    {
        return typeof(Queryable).GetMethods()
            .First(m => m.Name == "Average" &&
                m.GetParameters().Any(p =>
                    p.ParameterType.IsGenericType &&
                    p.ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                    p.ParameterType.GetGenericArguments()[0].IsGenericType &&
                    p.ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                    p.ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType));
    }

    private static MethodInfo DefaultIntAverage()
    {
        return FindAverageMethod(typeof(int));
    }

    private static MethodInfo DefaultInt64Average()
    {
        return FindAverageMethod(typeof(long));
    }

    private static MethodInfo DefaultFloatAverage()
    {
        return FindAverageMethod(typeof(float));
    }

    private static MethodInfo DefaultDoubleAverage()
    {
        return FindAverageMethod(typeof(double));
    }

    private static MethodInfo DefaultDecimalAverage()
    {
        return FindAverageMethod(typeof(decimal));
    }

}

public class Translator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeParser { get; }

    public Translator(TranslatorOptions? options = null)
    {
        Options = options ?? new();
        NodeParser = new(Options);
    }

    public Expression TranslateToQueryableExpression(Node node, Type type, ParameterExpression? parameter = null)
    {
        var queryableType = typeof(IEnumerable<>).MakeGenericType(type);
        parameter ??= Expression.Parameter(queryableType, "root");
        var context = new Context(queryableType, parameter);

        return NodeParser.Translate(context, node);
    }

    public TranslatedQueryable TranslateToQueryable(Node node, Type type, IEnumerable queryable)
    {
        var inputType = Options.QueryableType.MakeGenericType(type);
        var parameter = Expression.Parameter(inputType, "root");
        var context = new Context(inputType, parameter);
        var expression = NodeParser.Translate(context, node);

        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = Options.QueryableType.MakeGenericType(projectedType);
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

    public TranslatedQueryable TranslateToQueryable<T>(Node node, IEnumerable<T> queryable)
    {
        return TranslateToQueryable(node, typeof(T), queryable);
    }

}
