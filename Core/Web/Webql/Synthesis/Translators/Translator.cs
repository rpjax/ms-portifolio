using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Webql.Analysis;
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
    /// Represents an empty query.
    /// </summary>
    public const string EmptyQuery = "{}";

    /// <summary>
    /// Options for controlling the translation process. These options define the behavior and capabilities of the translation.
    /// </summary>
    public TranslatorOptions Options { get; }

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
    /// Translates a WebQL query, represented as a JSON string, into a LINQ expression. <br/>
    /// This method decodes the JSON-based WebQL query into a syntax tree and then translates it into a corresponding LINQ expression.
    /// It provides a direct way to transform WebQL queries into executable expressions without creating an IQueryable interface.
    /// </summary>
    /// <param name="json">The JSON representation of the WebQL query.</param>
    /// <param name="queryableType">The enumerable type that the query operates on.</param>
    /// <param name="rootParameter">An optional parameter expression serving as the root of the queryable expression tree.</param>
    /// <returns>A LINQ Expression representing the equivalent query logic for the given WebQL query.</returns>
    /// <exception cref="Exception">Thrown if the translation fails due to issues in syntax analysis or expression generation.</exception>
    public Expression TranslateToExpression(string json, Type queryableType, ParameterExpression? rootParameter = null)
    {
        var syntaxTree = RunAnalysis(json, queryableType);
        var exprParameter = rootParameter ?? Expression.Parameter(queryableType, "x");
        var context = new TranslationContext(queryableType, exprParameter);

        return NodeTranslator.Translate(context, syntaxTree);
    }

    /// <summary>
    /// Translates a WebQL query string into a queryable object. <br/>
    /// This method provides an integration point for executing WebQL queries against various data sources.
    /// </summary>
    /// <param name="json">The WebQL query string in JSON format.</param>
    /// <param name="genericType">The type of elements in the queryable.</param>
    /// <param name="queryable">The initial queryable object to which the WebQL query is applied.</param>
    /// <returns>A TranslatedQueryable object representing the results of the WebQL query.</returns>
    public TranslatedQueryable TranslateToQueryable(string json, Type genericType, IEnumerable queryable)
    {
        var queryableType = Options.QueryableType.MakeGenericType(genericType);
        var parameter = Expression.Parameter(queryableType, "root");
        var expression = TranslateToExpression(json, queryableType, parameter);
        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = Options.QueryableType.MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(queryableType, outputType);

        var lambdaExpression = Expression.Lambda(lambdaExpressionType, expression, parameter);
        var lambda = lambdaExpression.Compile();

        var transformedQueryable = lambda.DynamicInvoke(queryable);

        if (transformedQueryable == null)
        {
            throw QueryableTransformationFailedException();
        }

        return new TranslatedQueryable(queryableType.GenericTypeArguments.First(), outputType.GenericTypeArguments.Last(), transformedQueryable);
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

    /// <summary>
    /// Translates a WebQL query string into a queryable object. <br/>
    /// This method provides an integration point for executing WebQL queries against various data sources.
    /// </summary>
    /// <param name="expression">The expression tree representing the WebQL query logic.</param>
    /// <param name="genericType">The generic type argument of the queryable.</param>
    /// <param name="queryable">The initial queryable object to which the WebQL query is applied.</param>
    /// <returns>A TranslatedQueryable object representing the results of the WebQL query.</returns>
    /// <exception cref="Exception">Thrown if the transformation of the queryable fails.</exception>
    public TranslatedQueryable TranslateToQueryable(Expression expression, Type genericType, IEnumerable queryable)
    {
        var visitor = new ParameterExpressionReferenceBinder();

        var inputType = Options.QueryableType.MakeGenericType(genericType);
        var parameter = Expression.Parameter(inputType, "root");
        var projectedType = expression.Type.GenericTypeArguments[0];
        var outputType = Options.QueryableType.MakeGenericType(projectedType);
        var lambdaExpressionType = typeof(Func<,>).MakeGenericType(inputType, outputType);

        var lambdaExpression = visitor
            .Visit(Expression.Lambda(lambdaExpressionType, expression, parameter))
            .TypeCast<LambdaExpression>();

        var lambda = lambdaExpression.Compile();

        var transformedQueryable = lambda.DynamicInvoke(queryable);

        if (transformedQueryable == null)
        {
            throw QueryableTransformationFailedException();
        }

        return new TranslatedQueryable(inputType.GenericTypeArguments.First(), outputType.GenericTypeArguments.Last(), transformedQueryable);
    }

    /// <summary>
    /// Analyzes a JSON string representing a WebQL query and converts it into a syntax tree.
    /// </summary>
    /// <param name="json">The JSON string representing the WebQL query.</param>
    /// <param name="queryableType">The type of the root element in the query.</param>
    /// <returns>A Node representing the syntax tree of the query.</returns>
    public Node RunAnalysis(string json, Type queryableType)
    {
        return AnalysisPipeline.Run(json, queryableType);
    }

    /// <summary>
    /// Creates an exception to be thrown when the transformation of a queryable object fails.
    /// </summary>
    /// <param name="message">An optional message providing more details about the failure (default is null).</param>
    /// <returns>An Exception instance to be thrown.</returns>
    private Exception QueryableTransformationFailedException(string? message = null)
    {
        return new Exception($"Failed to transform queryable. {message}");
    }

}
