using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Webql;

/// <summary>
/// Provides helper methods to throw semantic exceptions with detailed error messages.
/// </summary>
public static class SemanticThrowHelper
{
    /// <summary>
    /// Throws a semantic exception indicating an unknown internal error, providing context and stack trace details.
    /// </summary>
    /// <param name="context">The semantic context where the error occurred.</param>
    /// <param name="message">A detailed message describing the error.</param>
    /// <param name="e">An optional inner exception that caused the current error.</param>
    /// <returns>A SemanticException that encapsulates the error details.</returns>
    public static Exception ErrorInternalUnknown(SemanticContext context, string? message, Exception? e = null)
    {
        var detailedMessage = string.IsNullOrEmpty(message)
            ? "An unknown internal error occurred during the semantic analysis process."
            : $"An unknown internal error occurred during the semantic analysis process: {message}.";

        return new SemanticException($"{detailedMessage} Please review the context and stack trace for more details. If the issue persists, consider seeking assistance or reporting this as a bug.", context, e);
    }

    // Additional helper methods can be added here to handle specific types of semantic exceptions.
    // For example, methods to handle invalid type references, syntax errors, or unsupported operations.
}

/// <summary>
/// A utility class for generating standardized exceptions related to the translation process in WebQL. <br/>
/// This class centralizes the creation of exceptions, ensuring consistent error messaging across different parts of the translation pipeline.
/// </summary>
public static class TranslationThrowHelper
{
    /// <summary>
    /// Creates a new TranslationException for internal errors that are not specifically categorized.
    /// </summary>
    /// <param name="context">The current translation context in which the error occurred.</param>
    /// <param name="message">A detailed message describing the error. If not provided, a default message is used.</param>
    /// <param name="e">An optional inner exception that caused this error, if applicable.</param>
    /// <returns>A TranslationException instance representing the unknown internal error.</returns>
    public static Exception ErrorInternalUnknown(TranslationContext context, string? message, Exception? e = null)
    {
        var detailedMessage = string.IsNullOrEmpty(message) ? "An unknown internal error occurred." : $"An unknown internal error occurred: {message}.";
        return new TranslationException($"{detailedMessage} Please review the context and stack trace for more details. If the issue persists, consider seeking assistance or reporting this as a bug.", context, e);
    }

    /// <summary>
    /// Generates a TranslationException for situations where an unexpected node type is encountered.
    /// </summary>
    /// <param name="context">The translation context in which the unexpected node type was encountered.</param>
    /// <param name="message">A message providing details about the unexpected node type. A default message is used if none is provided.</param>
    /// <returns>A TranslationException indicating that an unexpected node type was encountered.</returns>
    public static Exception WrongNodeType(TranslationContext context, string? message)
    {
        var detailedMessage = string.IsNullOrEmpty(message) ? "Encountered an unexpected node type." : $"Encountered an unexpected node type: {message}.";
        return new TranslationException($"{detailedMessage} Ensure that the node structure aligns with the expected format for WebQL queries. Review the query and context for potential mismatches.", context);
    }

    /// <summary>
    /// Generates an exception when a queryable-exclusive operator is used outside of a queryable context. <br/>
    /// This method helps to enforce the rule that certain operators in WebQL are exclusive to contexts where querying is applicable.
    /// </summary>
    /// <param name="context">The current translation context, providing context-specific information for the error.</param>
    /// <param name="operator">The operator that triggered the exception.</param>
    /// <returns>A TranslationException indicating the misuse of a queryable-exclusive operator.</returns>
    /// <remarks>
    /// Operators of type <see cref="OperatorType.Queryable"/> are intended for use only within contexts <br/>
    /// where they can operate on queryable data. This method assists in identifying and reporting such <br/>
    /// misuse in the translation process.
    /// </remarks>
    public static Exception QueryableExclusiveOperator(TranslationContext context, Operator @operator)
    {
        var operatorName = HelperTools.Stringify(@operator);
        return new TranslationException($"The '{operatorName}' operator is exclusive to queryable contexts and cannot be used in the current context. Ensure that this operator is applied in a part of the query where it can operate on a collection or queryable data type.", context);
    }

}
