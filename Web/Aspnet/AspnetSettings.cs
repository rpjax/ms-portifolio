namespace ModularSystem.Web;

/// <summary>
/// A static singleton class designed to hold global configurations for ASP.NET applications. <br/>
/// Allows setting application-wide properties that affect the behavior and response strategies of the application.
/// </summary>
public static class AspnetSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether exceptions should be exposed in the application's responses. <br/>
    /// When set to true, detailed exception information is included in API responses, useful for debugging. <br/>
    /// It is recommended to set this to false in production environments for security reasons.
    /// </summary>
    public static bool ExposeExceptions { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether errors marked as non-public should be exposed in the application's responses. <br/>
    /// This can be useful for providing detailed error information in a controlled internal environment.
    /// </summary>
    public static bool ExposeNonPublicErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets the HTTP status code returned for unhandled exceptions. <br/>
    /// Defaults to 500 (Internal Server Error), which is a common code for server-side error indications.
    /// </summary>
    public static int ExceptionStatusCode { get; set; } = 500;

    /// <summary>
    /// Gets or sets the HTTP status code returned for failed operations. <br/>
    /// Defaults to 417 (Expectation Failed), indicating that the server cannot meet the requirements of the Expect request-header field.
    /// </summary>
    public static int FailedOperationStatusCode { get; set; } = 417;
}
