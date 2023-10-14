using System.Text;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Represents an exception that is thrown when an error occurs during the parsing of an expression tree.
/// </summary>
/// <remarks>
/// This exception is used to indicate issues related to malformed serialized expressions that cannot be translated into their corresponding runtime expression types.
/// </remarks>
public class ParsingException : Exception
{
    /// <summary>
    /// Gets the detailed error message for this exception.
    /// </summary>
    public override string Message => _message;

    /// <summary>
    /// Gets the context in which the conversion error occurred.
    /// </summary>
    protected ConversionContext ParseContext { get; }

    private readonly string _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class with a specified error message, conversion context, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="context">The context in which the conversion error occurred.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    /// <param name="additionalData">Additional data related to the exception context.</param>
    public ParsingException(string message, ConversionContext context, Exception? innerException, object? additionalData = null)
        : base(message, innerException)
    {
        ParseContext = context;
        _message = CreateMessage(message);
        Data.Add("Context Additional Data", additionalData);
    }

    /// <summary>
    /// Constructs a detailed error message based on the provided message and the current parsing context.
    /// </summary>
    /// <param name="message">The base error message.</param>
    /// <returns>The detailed error message.</returns>
    private string CreateMessage(string message)
    {
        var strBuilder = new StringBuilder(150);

        message = message.Trim();

        strBuilder.Append("An error occurred while parsing the provided expression tree. This suggests that the serialized expression might be malformed and couldn't be translated to its corresponding runtime expression type. Detailed error: ");

        strBuilder.Append(message);

        if (!message.EndsWith("."))
            strBuilder.Append('.');

        strBuilder.Append(' ');
        strBuilder.Append($"Location in the stack: [{ParseContext.ToString()}].");

        return strBuilder.ToString();
    }
}
