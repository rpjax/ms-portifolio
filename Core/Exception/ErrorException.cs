using System.Linq;

namespace ModularSystem.Core;

/// <summary>
/// Represents an exception that encapsulates multiple errors. <br/>
/// This class extends the standard Exception class to include an array of <see cref="Error"/> objects, <br/>
/// allowing for detailed error reporting in scenarios where multiple errors need to be communicated.
/// </summary>
public class ErrorException : Exception
{
    /// <summary>
    /// Gets or sets an array of <see cref="Error"/> objects associated with this exception.
    /// </summary>
    public Error[] Errors { get; set; } = Array.Empty<Error>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorException"/> class with an array of <see cref="Error"/> objects.
    /// </summary>
    /// <param name="errors">An array of <see cref="Error"/> objects representing the errors that caused this exception.</param>
    /// <remarks>
    /// The message for this exception is derived from the first <see cref="Error"/> object in the array, if any.
    /// <br/>
    /// If the array is empty, the exception message is set to a default empty string.
    /// </remarks>
    public ErrorException(params Error[] errors) : base(errors.FirstOrDefault()?.ToString())
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorException"/> class with a collection of <see cref="Error"/> objects.
    /// </summary>
    /// <param name="errors">A collection of <see cref="Error"/> objects representing the errors that caused this exception.</param>
    /// <remarks>
    /// This constructor converts the provided collection into an array and delegates to the primary constructor.
    /// <br/>
    /// It provides a convenient way to initialize the exception with a collection of errors.
    /// </remarks>
    public ErrorException(IEnumerable<Error> errors) : this(errors.ToArray())
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorException"/> class with errors from an <see cref="IOperationResult"/>.
    /// </summary>
    /// <param name="operation">The <see cref="IOperationResult"/> containing the errors to be included in the exception.</param>
    /// <remarks>
    /// This constructor extracts the errors from the provided operation result and initializes the exception with these errors. <br/>
    /// It's useful for propagating errors from an operation result into an exception context.
    /// </remarks>
    public ErrorException(IOperationResult operation) : this(operation.Errors)
    {
    }

    public override string ToString()
    {
        var parts = new List<string>();
        var errors = string.Join(" <---> ", Errors.Select(e => e.ToString()));

        parts.Add($"ErrorException: {Environment.NewLine}");
        parts.Add(errors);

        return string.Join("", parts);
    }

}
