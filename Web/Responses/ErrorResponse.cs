using ModularSystem.Core;
using System.Text.Json.Serialization;

namespace ModularSystem.Web.Responses;

/// <summary>
/// Represents an error response based on RFC 7807 - Problem Details for HTTP APIs.
/// </summary>
public class ErrorResponse
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
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// </summary>
    [JsonConstructor]
    public ErrorResponse()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="title">The title of the error.</param>
    /// <param name="type">The type of the error. Default value is "about:blank".</param>
    /// <param name="detail">The detail of the error.</param>
    /// <param name="errors">The array of errors.</param>
    public ErrorResponse(
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
    /// Creates an <see cref="ErrorResponse"/> from a single <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>An <see cref="ErrorResponse"/> object.</returns>
    public static ErrorResponse FromError(Error error)
    {
        return new ErrorResponse(
            title: error.Title,
            detail: error.Description,
            errors: new Error[] { error }
        );
    }

    /// <summary>
    /// Creates an <see cref="ErrorResponse"/> from multiple <see cref="Error"/> objects.
    /// </summary>
    /// <param name="errors">The errors.</param>
    /// <returns>An <see cref="ErrorResponse"/> object.</returns>
    public static ErrorResponse FromErrors(params Error[] errors)
    {
        var title = errors.Length > 0 ? errors[0].Title : null;
        var detail = errors.Length > 0 ? errors[0].Description : null;

        return new ErrorResponse(
            title: title,
            detail: detail,
            errors: errors
        );
    }

    /// <summary>
    /// Creates an <see cref="ErrorResponse"/> from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>An <see cref="ErrorResponse"/> object.</returns>
    public static ErrorResponse FromException(Exception exception)
    {
        return FromError(Error.FromException(exception));
    }
}
