using System.Collections.Concurrent;
using System.Text;
using ModularSystem.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModularSystem.Core;

/// <summary>
/// Manages the loading and retrieval of environment configuration.
/// </summary>
public class EnvironmentConfig
{
    public const string DefaultFileName = "env.json";

    public static readonly MemorySize DefaultMaximumFileSize = MemorySize.MegaByte(5);

    protected FileInfo _fileInfo;
    protected MemorySize _maximumFileSize;
    protected JObject? _json;
    protected ConcurrentDictionary<string, object> _keyObjectDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentConfig"/> class.
    /// </summary>
    /// <param name="fileInfo">Information about the environment configuration file.</param>
    /// <param name="maximumSize">Maximum allowed size for the environment file.</param>
    public EnvironmentConfig(FileInfo fileInfo, MemorySize? maximumSize = null)
    {
        _fileInfo = fileInfo;
        _json = null;
        _keyObjectDictionary = new();
        _maximumFileSize = maximumSize ?? DefaultMaximumFileSize;
    }

    /// <summary>
    /// Loads the environment configuration using the specified file name.
    /// </summary>
    /// <param name="fileName">Name of the environment configuration file.</param>
    /// <returns>An initialized environment configuration.</returns>
    public static EnvironmentConfig Load(string fileName = DefaultFileName)
    {
        var filePath = FileSystemHelper.NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}{fileName}");
        var fileInfo = new FileInfo(filePath);

        return new EnvironmentConfig(fileInfo);
    }

    /// <summary>
    /// Loads and initializes the environment configuration with the provided type <typeparamref name="T"/> and the specified file name.
    /// </summary>
    /// <param name="fileName">Name of the environment configuration file.</param>
    /// <returns>An initialized environment configuration.</returns>
    public static EnvironmentConfig LoadInit<T>(string fileName = DefaultFileName) where T : class
    {
        var filePath = FileSystemHelper.NormalizeAbsolutePath($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}{fileName}");
        var fileInfo = new FileInfo(filePath);

        return new EnvironmentConfig(fileInfo).Init<T>();
    }

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
            throw new FileNotFoundException($"The environment file was not found. at: \"{fileInfo.FullName}\".");
        }
        if (fileInfo.Length > DefaultMaximumFileSize.SizeInBytes)
        {
            throw new Exception("The environment file exeeds the maximum allowed size set.");
        }

        new EnvironmentConfigValidator(typeof(T), fileInfo).Run();

        var text = File.ReadAllText(fileInfo.FullName);
        var obj = JsonConvert.DeserializeObject<T>(text);

        if (obj == null)
        {
            throw new Exception($"Could not deserialize the environment file into an instance of type \"{typeof(T).FullName}\".");
        }

        return obj;
    }

    /// <summary>
    /// Initializes the current environment configuration instance using <typeparamref name="T"/> as template.
    /// </summary>
    /// <typeparam name="T">The type representing the configuration structure.</typeparam>
    /// <returns>The initialized environment configuration.</returns>
    public EnvironmentConfig Init<T>() where T : class
    {
        _fileInfo.Refresh();

        if (!_fileInfo.Exists)
        {
            throw new FileNotFoundException($"The environment file was not found. at: \"{_fileInfo.FullName}\".");
        }
        if (_fileInfo.Length > _maximumFileSize.SizeInBytes)
        {
            throw new Exception("The environment file exeeds the maximum allowed size set.");
        }

        new EnvironmentConfigValidator(typeof(T), _fileInfo).Run();

        _json = JObject.Parse(File.ReadAllText(_fileInfo.FullName));
        MapToDictionary<T>(_json, _keyObjectDictionary);
        return this;
    }

    public FileInfo FileInfo()
    {
        return _fileInfo;
    }

    public bool Contains(string key)
    {
        return _keyObjectDictionary.ContainsKey(key.ToLower());
    }

    /// <summary>
    /// Tries to get an object from the environment configuration.
    /// </summary>
    /// <param name="key">The key of the object to retrieve.</param>
    /// <returns>The retrieved object or null.</returns>
    public object? TryGetObject(string key)
    {
        if (_keyObjectDictionary.TryGetValue(key.ToLower(), out var value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Gets an object from the environment configuration or throws an exception if not found.
    /// </summary>
    /// <param name="key">The key of the object to retrieve.</param>
    /// <returns>The retrieved object.</returns>
    public object GetObject(string key)
    {
        var obj = TryGetObject(key);

        if (obj == null)
        {
            throw new Exception($"Could not retrive the element \"{key}\" from environemnt file.");
        }

        return obj;
    }

    /// <summary>
    /// Tries to get an object of a specific type from the environment configuration.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <param name="key">The key of the object to retrieve.</param>
    /// <returns>The retrieved object or null.</returns>
    public T? TryGet<T>(string key) where T : class
    {
        var obj = TryGetObject(key);

        if (obj == null)
        {
            return null;
        }

        return obj.TryTypeCast<T>();
    }

    /// <summary>
    /// Gets an object of a specific type from the environment configuration or throws an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <param name="key">The key of the object to retrieve.</param>
    /// <returns>The retrieved object.</returns>
    public T Get<T>(string key) where T : class
    {
        var obj = TryGet<T>(key);

        if (obj == null)
        {
            throw new Exception($"Could not retrive the element \"{key}\" from environemnt file.");
        }

        return obj;
    }

    /// <summary>
    /// Maps the JSON content of the environment configuration to a dictionary.
    /// </summary>
    /// <typeparam name="T">The type representing the configuration structure.</typeparam>
    /// <param name="jobject">The JSON content.</param>
    /// <param name="dictionary">The dictionary to populate.</param>
    void MapToDictionary<T>(JObject jobject, IDictionary<string, object> dictionary) where T : class
    {
        var type = typeof(T);
        var jProps = jobject.Properties();
        var objProps = type.GetProperties();
        var groups = jProps.GroupBy(x => x.Name);
        var duplicatedProps = groups.Where(x => x.Count() > 1);

        if (duplicatedProps.IsNotEmpty())
        {
            var errorBuilder = new StringBuilder();
            var propNames = duplicatedProps.Select(x => x.First().Name).ToArray();

            errorBuilder.AppendLine("The loaded environment file contains duplicated properties: ");

            for (int i = 0; i < propNames.Length; i++)
            {
                var name = propNames[i];
                var isLast = i == propNames.Length - 1;

                if (isLast)
                {
                    errorBuilder.AppendLine($"{name}.");
                }
                else
                {
                    errorBuilder.AppendLine($"{name}, ");
                }
            }

            throw new Exception(errorBuilder.ToString());
        }


        foreach (var jProp in jProps)
        {
            var propInfoEnum = objProps.Where(_propInfo => _propInfo.Name.ToLower() == jProp.Name.ToLower());

            if (propInfoEnum.IsEmpty())
            {
                continue;
            }

            var propInfo = propInfoEnum.First();

            var key = jProp.Name;
            var value = jProp.Value.ToObject(propInfo.PropertyType);

            if (value == null)
            {
                continue;
            }

            dictionary.TryAdd(key, value);
        }
    }
}
