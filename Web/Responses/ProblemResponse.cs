using Aidan.Core;
using System.Text.Json.Serialization;

namespace Aidan.Web.Responses;

/// <summary>
/// Represents an error response based on RFC 7807 - Problem Details for HTTP APIs.
/// </summary>
public class ProblemResponse
{
    /// <summary>
    /// Gets or sets the title of the error.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the type of the error.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the detail of the error.
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Gets or sets the array of errors.
    /// </summary>
    public Error[]? Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemResponse"/> class.
    /// </summary>
    [JsonConstructor]
    public ProblemResponse()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="title">The title of the error.</param>
    /// <param name="type">The type of the error. Default value is "about:blank".</param>
    /// <param name="detail">The detail of the error.</param>
    /// <param name="errors">The array of errors.</param>
    public ProblemResponse(
        string? title,
        string? type = "about:blank",
        string? detail = null,
        IEnumerable<Error>? errors = null)
    {
        Type = type;
        Title = title;
        Detail = detail;
        Errors = errors?.ToArray();

        if (Title is null && Errors is not null && Errors.Length > 0)
        {
            Title = Errors[0].Title;
        }
    }

    /// <summary>
    /// Creates an <see cref="ProblemResponse"/> from a single <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>An <see cref="ProblemResponse"/> object.</returns>
    public static ProblemResponse FromError(Error error)
    {
        return new ProblemResponse(
            title: error.Title,
            detail: error.Description,
            errors: new Error[] { error }
        );
    }

    /// <summary>
    /// Creates an <see cref="ProblemResponse"/> from multiple <see cref="Error"/> objects.
    /// </summary>
    /// <param name="errors">The errors.</param>
    /// <returns>An <see cref="ProblemResponse"/> object.</returns>
    public static ProblemResponse FromErrors(IEnumerable<Error> errors)
    {
        var errorsArray = errors.ToArray();
        var title = errorsArray.Length > 0 ? errorsArray[0].Title : null;
        var detail = errorsArray.Length > 0 ? errorsArray[0].Description : null;

        return new ProblemResponse(
            title: title,
            detail: detail,
            errors: errors
        );
    }

    /// <summary>
    /// Creates an <see cref="ProblemResponse"/> from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>An <see cref="ProblemResponse"/> object.</returns>
    public static ProblemResponse FromException(Exception exception)
    {
        return FromError(Error.FromException(exception));
    }
}
