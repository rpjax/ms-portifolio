using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Analysis.SyntaxFeatures;
using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Provides mechanisms for translating WebQL nodes, represented as JSON strings, into queryable expressions. <br/>
/// This class serves as the core translator for WebQL, enabling the conversion of WebQL queries into equivalent LINQ expressions.
/// </summary>
public class Translator
{
    /// <summary>
    /// Options for controlling the translation process. These options define the behavior and capabilities of the translation.
    /// </summary>
    private TranslatorOptions Options { get; }

    /// <summary>
    /// The translator used for converting individual nodes to LINQ expressions. <br/>
    /// This component is responsible for the detailed translation of WebQL nodes.
    /// </summary>
    private NodeTranslator NodeTranslator { get; }

    /// <summary>
    /// Initializes a new instance of the Translator class with optional settings. <br/>
    /// This constructor allows the customization of the translation process through various options.
    /// </summary>
    /// <param name="options">Optional settings for the translation process.</param>
    public Translator(TranslatorOptions? options = null)
    {
        Options = options ?? new TranslatorOptions();
        NodeTranslator = new NodeTranslator(Options);
    }

    /// <summary>
    /// Translates a WebQL query, represented as a JSON string, into a LINQ queryable expression. <br/>
    /// This method forms the bridge between JSON-based WebQL queries and LINQ expressions.
    /// </summary>
    /// <param name="json">The JSON representation of the WebQL query.</param>
    /// <param name="type">The type of elements in the resulting queryable.</param>
    /// <param name="parameter">An optional parameter expression serving as the root of the queryable expression tree.</param>
    /// <returns>A LINQ expression that represents the equivalent queryable for the given WebQL query.</returns>
    public Expression TranslateToQueryableExpression(string json, Type type, ParameterExpression? parameter = null)
    {
        var node = RunSyntaxAnalysis(json, type);
        var inputType = Options.QueryableType.MakeGenericType(type);
        parameter ??= Expression.Parameter(inputType, "root");
        var context = new TranslationContext(inputType, parameter);
        return NodeTranslator.Translate(context, node);
    }

    /// <summary>
    /// Translates a WebQL query string into a queryable object. <br/>
    /// This method provides an integration point for executing WebQL queries against various data sources.
    /// </summary>
    /// <param name="json">The WebQL query string in JSON format.</param>
    /// <param name="type">The type of elements in the queryable.</param>
    /// <param name="queryable">The initial queryable object to which the WebQL query is applied.</param>
    /// <returns>A TranslatedQueryable object representing the results of the WebQL query.</returns>
    public TranslatedQueryable TranslateToQueryable(string json, Type type, IEnumerable queryable)
    {
        var inputType = Options.QueryableType.MakeGenericType(type);
        var parameter = Expression.Parameter(inputType, "root");
        var expression = TranslateToQueryableExpression(json, type, parameter);
        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = Options.QueryableType.MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(inputType, outputType);

        var lambdaExpression = Expression.Lambda(lambdaExpressionType, expression, parameter);
        var lambda = lambdaExpression.Compile();

        var transformedQueryable = lambda.DynamicInvoke(queryable);

        if (transformedQueryable == null)
        {
            throw new Exception("Failed to transform queryable.");
        }

        return new TranslatedQueryable(inputType.GenericTypeArguments.First(), outputType.GenericTypeArguments.Last(), transformedQueryable);
    }

    /// <summary>
    /// Translates a WebQL query string into a queryable object for a specified type. <br/>
    /// This method simplifies the process of translating WebQL queries for specific data types.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queryable.</typeparam>
    /// <param name="json">The WebQL query string in JSON format.</param>
    /// <param name="queryable">The initial queryable object of type T.</param>
    /// <returns>A TranslatedQueryable object representing the results of the WebQL query for the specified type.</returns>
    public TranslatedQueryable TranslateToQueryable<T>(string json, IEnumerable<T> queryable)
    {
        return TranslateToQueryable(json, typeof(T), queryable);
    }

    private Node RunSyntaxAnalysis(string json, Type type)
    {
        var node = SyntaxAnalyser.Parse(json);
        var context = new SemanticContext(type);

        node = new RelationalOperatorsSyntaxFeature()
            .Visit(context, node)
            .As<ObjectNode>();

        return node;
    }

}
