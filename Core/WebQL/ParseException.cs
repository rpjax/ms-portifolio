namespace ModularSystem.WebQL;

public class ParseException : Exception
{
    public ParseStackTrace ParseStackTrace { get; init; }

    public ParseException(string message, ParseStackTrace stackTrace, Exception? inner = null) : base(Error(message, stackTrace), inner)
    {
        ParseStackTrace = stackTrace;
    }

    static string Error(string message, ParseStackTrace stackTrace)
    {
        return $"{message} Error at: '{stackTrace.ToString()}'.";
    }
}