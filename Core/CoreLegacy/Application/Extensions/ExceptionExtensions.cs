using System.Text.Json;

namespace ModularSystem.Core;

public static class ExceptionExtensions
{
    /// <summary>
    /// Converts the given <see cref="Exception"/> to a <see cref="SerializableException"/>.
    /// </summary>
    /// <param name="e">The <see cref="Exception"/> instance to be converted.</param>
    /// <returns>Returns an instance of <see cref="SerializableException"/> based on the provided <see cref="Exception"/>.</returns>
    public static SerializableException ToSerializable(this Exception e)
    {
        return new SerializableException(e);
    }

    public static AppException ToAppException(this Exception e)
    {
        if (e is AppException)
        {
            return (AppException)e;
        }

        return new AppException(e.Message, ExceptionCode.Internal, e);
    }

    /// <summary>
    /// Converts the <see cref="Exception"/> to a JSON string representation. 
    /// </summary>
    /// <remarks>
    /// Warning! This method serializes and exposes all exception data into the resulting JSON. <br/>
    /// It's important to ensure that the resulting JSON is not sent to untrusted parties to avoid potential information leaks.
    /// </remarks>
    /// <param name="e">The <see cref="Exception"/> instance to be converted to JSON.</param>
    /// <param name="settings">Optional <see cref="JsonSerializerOptions"/> to customize the serialization process.</param>
    /// <returns>Returns a JSON string representation of the <see cref="Exception"/>.</returns>
    public static string ToJson(this Exception e, JsonSerializerOptions? settings = null)
    {
        return e.ToSerializable().ToJson(settings);
    }

    public static string ToFullString(this Exception e)
    {
        return $"{e.Message} - {e.StackTrace}";
    }

    public static string ToFullStringRecursive(this Exception e)
    {
        var text = e.ToFullString();

        if (e.InnerException != null)
        {
            text = $"{text} <= {ToFullStringRecursive(e.InnerException)}";
        }

        return text;
    }
}
