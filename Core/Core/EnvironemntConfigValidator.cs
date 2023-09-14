using Newtonsoft.Json.Linq;

namespace ModularSystem.Core;

/// <summary>
/// Validates a JSON configuration file against a specified type.
/// </summary>
public class EnvironmentConfigValidator
{
    /// <summary>
    /// The target type against which the JSON file is validated.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The file info of the JSON file to be validated.
    /// </summary>
    public FileInfo FileInfo { get; }

    public EnvironmentConfigValidator(Type type, FileInfo fileInfo)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
    }

    /// <summary>
    /// Executes the validation process.
    /// </summary>
    public void Run()
    {
        var json = File.ReadAllText(FileInfo.FullName);
        var obj = JObject.Parse(json);

        Validate(obj, Type);

        if (obj == null)
        {
            throw new InvalidOperationException($"Failed to deserialize the environment file '{FileInfo.FullName}' into type '{Type.FullName}'.");
        }
    }

    /// <summary>
    /// Validates the JObject against the provided type.
    /// </summary>
    /// <param name="jobject">The JSON object to validate.</param>
    /// <param name="type">The target type.</param>
    private void Validate(JObject jobject, Type type)
    {
        var typeProps = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var propertyInfo in typeProps)
        {
            var jprops = jobject.Properties()
                .Where(x => x.Name.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var propertyType = propertyInfo.PropertyType;

            if (!jprops.Any())
            {
                if (IsNullable(propertyType))
                {
                    continue;
                }

                throw new MissingFieldException($"Environment configuration lacks required property '{propertyInfo.Name}'.");
            }

            var jprop = jprops.First();

            if (IsUserDefinedType(propertyType))
            {
                if (jprop.Value.Type != JTokenType.Object)
                {
                    throw new InvalidDataException($"Expected '{propertyInfo.Name}' to be of object type in the configuration but found {jprop.Value.Type}.");
                }

                var propertyJobject = jprop.Value.ToObject<JObject>();
                Validate(propertyJobject, propertyType);
            }
        }
    }

    /// <summary>
    /// Determines if the given type is nullable.
    /// </summary>
    private bool IsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Determines if a type is user-defined.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is user-defined; otherwise, false.</returns>
    private bool IsUserDefinedType(Type type)
    {
        if (type.IsPrimitive || type.IsEnum || type.Namespace?.StartsWith("System") == true)
        {
            return false;
        }

        return true;
    }
}
