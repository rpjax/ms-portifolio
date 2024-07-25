using ModularSystem.Core.Helpers;
using Newtonsoft.Json;

namespace ModularSystem.Core;

/// <summary>
/// Manages the loading and retrieval of environment configuration.
/// </summary>
public static class ConfigurationFile
{
    public const string DefaultFileName = "config.json";

    /// <summary>
    /// Loads and deserializes the environment configuration into a specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration into.</typeparam>
    /// <param name="fileName">Name of the environment configuration file.</param>
    /// <returns>The deserialized configuration.</returns>
    public static T Load<T>(string fileName = DefaultFileName) where T : class
    {
        var filePath = FileSystemHelper.NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}{fileName}");
        var fileInfo = new FileInfo(filePath);

        fileInfo.Refresh();

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"The configuration file was not found. at: \"{fileInfo.FullName}\".");
        }

        var validator = new ConfigurationFileValidator(typeof(T), fileInfo);
        validator.Run();

        var text = File.ReadAllText(fileInfo.FullName);
        var obj = JsonConvert.DeserializeObject<T>(text);

        if (obj == null)
        {
            throw new Exception($"Could not deserialize the configuration file into an instance of type \"{typeof(T).FullName}\".");
        }

        return obj;
    }

}
