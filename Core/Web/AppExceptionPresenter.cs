using ModularSystem.Core;
using System.Text.Json;

namespace ModularSystem.Web;

public static class AppExceptionPresenter
{
    public const string DefaultInternalMessage = "An unexpected error has occurred in the server.";

    public static ExceptionCode GetExceptionCodeFrom(int code)
    {
        switch (code)
        {
            case 400:
                return ExceptionCode.BadRequest;
            case 422:
                return ExceptionCode.InvalidInput;
            case 401:
                return ExceptionCode.Unauthorized;
            case 403:
                return ExceptionCode.Forbidden;
            default:
                return ExceptionCode.Internal;
        }
    }

    public static int GetStatusCodeFrom(AppException e)
    {
        var code = e.Code;

        if (code == ExceptionCode.BadRequest)
        {
            return 400;
        }
        else if (code == ExceptionCode.InvalidInput)
        {
            return 422;
        }
        else if (code == ExceptionCode.Unauthorized)
        {
            return 401;
        }
        else if (code == ExceptionCode.CredentialsExpired || code == ExceptionCode.CredentialsInvalid)
        {
            return 401;
        }
        else if (code == ExceptionCode.Forbidden)
        {
            return 403;
        }
        else
        {
            return 500;
        }
    }

    public static string GetMessageFrom(AppException e)
    {
        var code = e.Code;

        if (code == ExceptionCode.BadRequest)
        {
            return MsgIsValid(e) ? e.Message : "Bad request, something is malformed in the request.";
        }
        else if (code == ExceptionCode.InvalidInput)
        {
            return MsgIsValid(e) ? e.Message : "Invalid input, something is wrong in the incoming data.";
        }
        else if (code == ExceptionCode.Unauthorized)
        {
            return MsgIsValid(e) ? e.Message : "Unathorized!";
        }
        else if (code == ExceptionCode.CredentialsExpired)
        {
            return MsgIsValid(e) ? e.Message : "Credentials expired.";
        }
        else if (code == ExceptionCode.CredentialsInvalid)
        {
            return MsgIsValid(e) ? e.Message : "Credentials invalid!";
        }
        else if (code == ExceptionCode.Forbidden)
        {
            return MsgIsValid(e) ? e.Message : "Forbbiden resource.";
        }
        else
        {
            return "An unexpected error has occurred in the server.";
        }
    }

    public static string ToJson(AppException e, bool hideInternal = true)
    {
        var presentedException = new PresentedAppException(e.Message, e.Code);

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

    static bool MsgIsValid(AppException e)
    {
        return e.Message != null && e.Message?.Length != 0;
    }
}

/// <summary>
/// A simplified version of the <see cref="AppException"/> that can be exposed to the web.
/// </summary>
public class PresentedAppException
{
    public string Message { get; set; }
    public ExceptionCode Code { get; set; }

    public PresentedAppException(string message, ExceptionCode code)
    {
        Message = message;
        Code = code;
    }
}