using ModularSystem.Core;
using System.Text.Json;

namespace ModularSystem.Web;

public static class AppExceptionPresenter
{
    public const string DefaultInternalMessage = "An unexpected error has occurred in the server.";

    public static int GetStatusCodeFrom(AppException e)
    {
        switch (e.Code)
        {
            case ExceptionCode.Internal:
                return 500;
            case ExceptionCode.BadRequest:
                return 400;
            case ExceptionCode.InvalidInput:
                return 422;
            case ExceptionCode.Unauthenticated:
                return 401;
            case ExceptionCode.Unauthorized:
                return 401;
            case ExceptionCode.CredentialsExpired:
                return 401;
            case ExceptionCode.CredentialsInvalid:
                return 401;
            case ExceptionCode.Forbidden:
                return 403;
            case ExceptionCode.InvalidState:
                return 409;
            case ExceptionCode.NotSupported:
                return 500;
            default:
                return 500;
        }
    }

    public static string ToJson(AppException e, bool hideInternal = true)
    {
        var presentedException = new PresentedAppException(e.Message, e.Code, e.Details);

        if (presentedException.Code == ExceptionCode.Internal && hideInternal)
        {
            presentedException.Message = DefaultInternalMessage;
        }

        return JsonSerializerSingleton.Serialize(presentedException, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            WriteIndented = true
        });
    }

}

/// <summary>
/// A simplified version of the <see cref="AppException"/> that can be exposed to the web.
/// </summary>
public class PresentedAppException
{
    public string Message { get; set; }
    public ExceptionCode Code { get; set; }
    public List<KeyValuePair<string, string>>? Details { get; set; } 

    public PresentedAppException(string message, ExceptionCode code, List<KeyValuePair<string, string>>? details = null)
    {
        Message = message;
        Details = details;
        Code = code;
    }
}