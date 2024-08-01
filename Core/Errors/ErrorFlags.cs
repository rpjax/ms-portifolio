namespace ModularSystem.Core;

/// <summary>
/// Provides a set of constants representing different categories of errors. <br/>
/// These flags are used to classify errors based on their visibility, severity, and purpose.
/// </summary>
public static class ErrorFlags
{
    /// <summary>
    /// Indicates that the error can be displayed to the user, and should not contain sensitive information. <br/>
    /// </summary>
    public const string UserVisible = "user_visible";

    /// <summary>
    /// Indicates that the error is relevant for debugging purposes. <br/>
    /// This flag can be used to classify errors that are useful for developers during the debugging process.
    /// It might contain technical details relevant for troubleshooting.
    /// </summary>
    public const string Debug = "debug";

    /// <summary>
    /// Marks the error as critical. <br/>
    /// This flag is used to denote errors of a severe nature, often indicating major failures
    /// or issues that require immediate attention.
    /// </summary>
    public const string Critical = "critical";

    /// <summary>
    /// Denotes that the error is derived from an exception. <br/>
    /// This flag can be used to indicate that the error encapsulates or is directly related to an exception, <br/>
    /// and typically contains detailed information about the exception. It helps in distinguishing <br/>
    /// standard operational errors from those specifically arising from exceptions.
    /// </summary>
    public const string Exception = "exception";

    /// <summary>
    /// Identifies the error as a bug. <br/>
    /// This flag is used to categorize errors as bugs, implying defects or problems in the code or logic. <br/>
    /// It indicates that the error is a result of a flaw in the system that may require a code review or a fix.
    /// </summary>
    public const string Bug = "bug";
}
