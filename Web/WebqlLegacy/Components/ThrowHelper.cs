using Aidan.Webql.Analysis;
using Aidan.Webql.Synthesis;

namespace Aidan.Webql;

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
    public static Exception ErrorInternalUnknown(SemanticContextOld context, string? message, Exception? e = null)
    {
        var detailedMessage = string.IsNullOrEmpty(message)
            ? "An unknown internal error occurred during the semantic analysis process."
            : $"An unknown internal error occurred during the semantic analysis process: {message}.";

        return new SemanticException($"{detailedMessage} Please review the context and stack trace for more details. If the issue persists, consider seeking assistance or reporting this as a bug.", context, e);
    }

    /// <summary>
    /// Generates a SemanticException for cases where an operator is either unknown or unsupported within the semantic context.
    /// </summary>
    /// <param name="context">The semantic context in which the operator is being used.</param>
    /// <param name="operatorName">The name of the operator that is unrecognized or unsupported.</param>
    /// <returns>A SemanticException indicating the problem with the operator.</returns>
    /// <remarks>
    /// This method is useful for scenarios where an operator might be misspelled, misused, or simply not applicable within the current context. <br/>
    /// It provides a clear indication that the operator in question does not align with the semantic rules or expectations of the context.
    /// </remarks>
    public static Exception UnknownOrUnsupportedOperator(SemanticContextOld context, string operatorName)
    {
        var detailedMessage = $"The operator '{operatorName}' is either unknown or unsupported in the current semantic context.";
        return new SemanticException($"{detailedMessage} Ensure that the operator is valid and applicable within the specific context of the query. If the operator name is correct, verify that the context supports its usage.", context);
    }


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
    public static Exception ErrorInternalUnknown(TranslationContextOld context, string? message, Exception? e = null)
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
    public static Exception WrongNodeType(TranslationContextOld context, string? message)
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
    /// Operators of type <see cref="OperatorTypeOld.Queryable"/> are intended for use only within contexts <br/>
    /// where they can operate on queryable data. This method assists in identifying and reporting such <br/>
    /// misuse in the translation process.
    /// </remarks>
    public static Exception QueryableExclusiveOperator(TranslationContextOld context, OperatorOld @operator)
    {
        var operatorName = WebqlHelper.Stringify(@operator);
        return new TranslationException($"The '{operatorName}' operator is exclusive to queryable contexts and cannot be used in the current context. Ensure that this operator is applied in a part of the query where it can operate on a collection or queryable data type.", context);
    }

    /// <summary>
    /// Generates a TranslationException when an incorrect number of arguments are passed to an operator.
    /// This method assists in enforcing argument count constraints in the translation process.
    /// </summary>
    /// <param name="context">The translation context, providing relevant details about where the error occurred.</param>
    /// <param name="message">An optional message providing additional details about the error. If not provided, a default message is used.</param>
    /// <returns>A TranslationException indicating the issue with the argument count.</returns>
    public static Exception WrongArgumentsCount(TranslationContextOld context, string? message)
    {
        var detailedMessage = string.IsNullOrEmpty(message)
            ? "An invalid number of arguments were passed to an operator."
            : $"An invalid number of arguments were passed to an operator: {message}.";
        return new TranslationException($"{detailedMessage} Ensure that the correct number of arguments are used as per the operator's definition.", context);
    }

    /// <summary>
    /// Generates a TranslationException for incorrect argument counts in array syntax with binary operators.
    /// This method is specifically aimed at handling errors in binary operator usage within array contexts.
    /// </summary>
    /// <param name="context">The translation context, providing context-specific details for the error.</param>
    /// <param name="message">An optional message providing more details about the error. If not provided, a default message is used.</param>
    /// <returns>A TranslationException highlighting the improper use of binary operators in array syntax.</returns>
    public static Exception ArraySyntaxBinaryExprWrongArgumentsCount(TranslationContextOld context, string? message)
    {
        var detailedMessage = string.IsNullOrEmpty(message)
            ? "An invalid number of arguments were passed to a binary operator within an array syntax."
            : $"An invalid number of arguments were passed to a binary operator within an array syntax: {message}.";
        return new TranslationException($"{detailedMessage} The correct syntax for binary operators within arrays should follow the format: {{ $operator: [<arg1>, <arg2>] }}", context);
    }

    /// <summary>
    /// Generates a TranslationException when an operator receives an argument of an incorrect or unexpected type.
    /// This method helps to enforce argument type constraints in the translation process.
    /// </summary>
    /// <param name="context">The translation context, providing relevant details about where the error occurred.</param>
    /// <param name="message">An optional message providing additional details about the error. If not provided, a default message is used.</param>
    /// <returns>A TranslationException indicating the issue with the argument type.</returns>
    public static Exception WrongArgumentType(TranslationContextOld context, string? message)
    {
        var detailedMessage = string.IsNullOrEmpty(message)
            ? "An argument of an incorrect or unexpected type was passed to an operator."
            : $"An argument of an incorrect or unexpected type was passed to an operator: {message}.";
        return new TranslationException($"{detailedMessage} Ensure that all arguments match the expected types as per the operator's definition.", context);
    }

    /// <summary>
    /// Generates a TranslationException when an operator is either unknown or not supported in the translation context.
    /// </summary>
    /// <param name="context">The current translation context in which the error occurred.</param>
    /// <param name="operatorName">The name of the operator causing the issue.</param>
    /// <returns>A TranslationException indicating the operator's incompatibility or non-existence in the context.</returns>
    /// <remarks>
    /// This method addresses issues related to operator validity in the translation process. <br/>
    /// It's particularly helpful for detecting and informing about operators that are either incorrectly identified <br/>
    /// or not suitable for the given translation context, thus aiding in debugging and query refinement.
    /// </remarks>
    public static Exception UnknownOrUnsupportedOperator(TranslationContextOld context, string operatorName)
    {
        var detailedMessage = $"The operator '{operatorName}' is either unknown or unsupported in the current translation context.";
        return new TranslationException($"{detailedMessage} Check the operator's validity and ensure it's appropriate for the translation context. Review the query and the operator's applicability to the types and structures involved.", context);
    }

    public static Exception UnknownOrUnsupportedOperator(TranslationContextOld context, OperatorOld @operator)
    {
        return UnknownOrUnsupportedOperator(context, WebqlHelper.Stringify(@operator));
    }

    public static TranslationException IncorrectReturnType(
        TranslationContextOld context,
        Type expectedType,
        Type encounteredType
    )
    {
        return new TranslationException("", context);
    }

}
