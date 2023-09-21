namespace ModularSystem.Core;

/// <summary>
/// Represents standardized error codes for the application, capturing various types of errors and their meanings.
/// </summary>
public enum ExceptionCode
{
    /// <summary>
    /// Represents an unexpected internal error within the application. Details of this error should typically<br/>
    /// not be exposed to end users for security reasons.
    /// </summary>
    Internal,

    /// <summary>
    /// Indicates the request was malformed. Typically, this means the structure, syntax, or composition<br/>
    /// of the provided data is incorrect.
    /// </summary>
    BadRequest,

    /// <summary>
    /// Indicates the input was syntactically valid but semantically incorrect. For instance, the provided <br/>
    /// data might be outside the expected range or doesn't meet certain validation criteria.
    /// </summary>
    InvalidInput,

    /// <summary>
    /// Indicates the request lacks valid authentication credentials.
    /// </summary>
    Unauthenticated,

    /// <summary>
    /// Indicates the request lacks the required permissions for execution. <br/>
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The credentials provided with the request have expired and are no longer valid. <br/>
    /// The requester might need to renew their authentication token or session.
    /// </summary>
    CredentialsExpired,

    /// <summary>
    /// The credentials provided with the request are not recognized or are incorrect, and thus, the request cannot<br/>
    /// be authenticated.
    /// </summary>
    CredentialsInvalid,

    /// <summary>
    /// Indicates the request was attempting to access a resource or perform an action that it's not allowed to.<br/>
    /// This could be due to user-level permissions or application-level restrictions.
    /// </summary>
    Forbidden
}