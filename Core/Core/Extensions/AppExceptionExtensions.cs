using System.Text.Json;

namespace ModularSystem.Core;

/// <summary>
/// Provides extension methods for the <see cref="AppException"/> class.
/// </summary>
public static class AppExceptionExtensions
{
    /// <summary>
    /// Converts the given <see cref="AppException"/> to a <see cref="SerializableAppException"/>.
    /// </summary>
    /// <param name="e">The <see cref="AppException"/> instance to be converted.</param>
    /// <returns>Returns an instance of <see cref="SerializableAppException"/> based on the provided <see cref="AppException"/>.</returns>
    public static SerializableAppException ToSerializableAppException(this AppException e)
    {
        return new SerializableAppException(e);
    }

    /// <summary>
    /// Converts the <see cref="AppException"/> to a JSON string representation. 
    /// </summary>
    /// <remarks>
    /// Warning! This method serializes and exposes all exception data into the resulting JSON. <br/>
    /// It's important to ensure that the resulting JSON is not sent to untrusted parties to avoid potential information leaks.
    /// </remarks>
    /// <param name="e">The <see cref="AppException"/> instance to be converted to JSON.</param>
    /// <param name="settings">Optional <see cref="JsonSerializerOptions"/> to customize the serialization process.</param>
    /// <returns>Returns a JSON string representation of the <see cref="AppException"/>.</returns>
    public static string ToJson(this AppException e, JsonSerializerOptions? settings = null)
    {
        return e.ToSerializableAppException().ToJson(settings);
    }
}
