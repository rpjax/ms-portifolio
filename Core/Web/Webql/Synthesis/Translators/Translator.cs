using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Provides mechanisms for translating WebQL nodes into queryable expressions.
/// </summary>
public class Translator
{
    /// <summary>
    /// Options for controlling the translation process.
    /// </summary>
    private TranslatorOptions Options { get; }

    /// <summary>
    /// The translator used for translating individual nodes to LINQ expressions.
    /// </summary>
    private NodeTranslator NodeTranslator { get; }

    /// <summary>
    /// Initializes a new instance of the Translator class with optional settings.
    /// </summary>
    /// <param name="options">Optional translation settings.</param>
    public Translator(TranslatorOptions? options = null)
    {
        Options = options ?? new();
        NodeTranslator = new(Options);
    }

    /// <summary>
    /// Translates a WebQL node into a LINQ queryable expression.
    /// </summary>
    /// <param name="node">The node to translate.</param>
    /// <param name="type">The type of elements in the queryable.</param>
    /// <param name="parameter">Optional parameter expression for the root of the queryable.</param>
    /// <returns>A LINQ expression representing the queryable.</returns>
    public Expression TranslateToQueryableExpression(Node node, Type type, ParameterExpression? parameter = null)
    {
        var queryableType = typeof(IEnumerable<>).MakeGenericType(type);
        parameter ??= Expression.Parameter(queryableType, "root");
        var context = new Context(queryableType, parameter);

        return NodeTranslator.Translate(context, node);
    }

    /// <summary>
    /// Translates a WebQL node into a queryable object.
    /// </summary>
    /// <param name="node">The node to translate.</param>
    /// <param name="type">The type of elements in the queryable.</param>
    /// <param name="queryable">The initial queryable object.</param>
    /// <returns>A TranslatedQueryable representing the resulting query.</returns>
    public TranslatedQueryable TranslateToQueryable(Node node, Type type, IEnumerable queryable)
    {
        var inputType = Options.QueryableType.MakeGenericType(type);
        var parameter = Expression.Parameter(inputType, "root");
        var context = new Context(inputType, parameter);
        var expression = NodeTranslator.Translate(context, node);

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

    /// <summary>
    /// Translates a WebQL node into a queryable object.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queryable.</typeparam>
    /// <param name="node">The node to translate.</param>
    /// <param name="queryable">The initial queryable object.</param>
    /// <returns>A TranslatedQueryable representing the resulting query.</returns>
    public TranslatedQueryable TranslateToQueryable<T>(Node node, IEnumerable<T> queryable)
    {
        return TranslateToQueryable(node, typeof(T), queryable);
    }

}
