using Aidan.Core;
using Aidan.Core.Linq;
using Aidan.Web.Webql.Synthesis.Productions;
using Aidan.Web.Webql.Synthesis.Symbols;
using Aidan.Webql.Analysis;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Provides mechanisms for translating WebQL nodes, represented as JSON strings, into queryable expressions. <br/>
/// This class serves as the core translator for WebQL, enabling the conversion of WebQL queries into equivalent LINQ expressions.
/// </summary>
public class Translator : TranslatorBase
{
    /// <summary>
    /// Represents an empty query.
    /// </summary>
    public const string EmptyQuery = "{}";

    /// <summary>
    /// Initializes a new instance of the Translator class with optional settings. <br/>
    /// This constructor allows the customization of the translation process through various options.
    /// </summary>
    /// <param name="options">Optional settings for the translation process.</param>
    public Translator(TranslationOptions? options = null) : base(options ?? new TranslationOptions())
    {

    }

    public Expression Translate(string query, Type queryableType)
    {
        var syntaxTree = RunAnalysis(query);

        var queryableParameter = Expression
            .Parameter(queryableType, "root");

        var context = new TranslationContextOld(new AxiomProduction());
        context.SetSymbol("$", queryableParameter, true);

        var axiom = TypeCastNode<ObjectNode>(context, syntaxTree);

        var expression = TranslateAxiom(context, axiom);
        var queryableExpression = new QueryArgumentExpression(expression);

        var projectedElementType = queryableExpression.GetElementType(context);

        var projectedQueryableType = queryableType
            .GetGenericTypeDefinition()
            .MakeGenericType(projectedElementType);

        var lambdaExpressionType = typeof(Func<,>)
            .MakeGenericType(queryableType, projectedQueryableType);

        var lambdaExpression = Expression.Lambda(lambdaExpressionType, expression, queryableParameter);
        
        return lambdaExpression;
    }

    /// <summary>
    /// Translates a WebQL query and applies it to a given IQueryable source.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the IQueryable source.</typeparam>
    /// <param name="query">The WebQL query string in JSON format.</param>
    /// <param name="source">The IQueryable source to which the WebQL query will be applied.</param>
    /// <returns>An IQueryable object representing the results of the WebQL query.</returns>
    public IQueryable<object> Translate<T>(string query, IQueryable<T> source)
    {
        var expression = Translate(query, typeof(IQueryable<T>));
        var lambdaExpression = expression.TypeCast<LambdaExpression>();
        var lambda = lambdaExpression.Compile();

        var transformedSource = lambda.DynamicInvoke(source) as IQueryable;

        if (transformedSource == null)
        {
            throw QueryableTransformationFailedException();
        }

        return new WebqlQueryable(transformedSource);
    }

    /// <summary>
    /// Translates a WebQL query and applies it to a given IAsyncQueryable source.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the IAsyncQueryable source.</typeparam>
    /// <param name="query">The WebQL query string in JSON format.</param>
    /// <param name="source">The IAsyncQueryable source to which the WebQL query will be applied.</param>
    /// <returns>An IAsyncQueryable object representing the results of the WebQL query.</returns>
    public IAsyncQueryable<object> Translate<T>(string query, IAsyncQueryable<T> source)
    {
        var expression = Translate(query, typeof(IAsyncQueryable<T>));
        var lambdaExpression = expression.TypeCast<LambdaExpression>();
        var lambda = lambdaExpression.Compile();

        var transformedSource = lambda.DynamicInvoke(source) as IAsyncQueryable;

        if (transformedSource == null)
        {
            throw QueryableTransformationFailedException();
        }

        return new WebqlAsyncQueryable(transformedSource);
    }

    /// <summary>
    /// Analyzes a JSON string representing a WebQL query and converts it into a syntax tree.
    /// </summary>
    /// <param name="json">The JSON string representing the WebQL query.</param>
    /// <returns>A Node representing the syntax tree of the query.</returns>
    public Node RunAnalysis(string json)
    {
        return AnalysisPipeline.Run(json);
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
