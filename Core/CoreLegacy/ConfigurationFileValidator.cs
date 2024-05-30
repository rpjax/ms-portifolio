using ModularSystem.Core.Json.Attributes;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ModularSystem.Core;

/// <summary>
/// Validates a JSON configuration file against a specified type.
/// </summary>
public class ConfigurationFileValidator
{
    /// <summary>
    /// The target type against which the JSON file is validated.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The file info of the JSON file to be validated.
    /// </summary>
    public FileInfo FileInfo { get; }

    public ConfigurationFileValidator(Type type, FileInfo fileInfo)
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
            throw new InvalidOperationException($"Failed to deserialize the configuration file '{FileInfo.FullName}' into type '{Type.FullName}'.");
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
                if (PropertyIsNullable(propertyInfo))
                {
                    continue;
                }

                throw new MissingFieldException($"Configuration file lacks required property '{propertyInfo.Name}'.");
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
    /// Determines if the given property is nullable.
    /// </summary>
    private bool PropertyIsNullable(PropertyInfo propertyInfo)
    {
        // Verifica se é um tipo de valor nullable
        if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
        {
            return true;
        }

        // Verifica se é um tipo de referência nullable
        if (!propertyInfo.PropertyType.IsValueType)
        {
            var nullableAttribute = propertyInfo.GetCustomAttribute<NullableAttribute>();

            if (nullableAttribute != null)
            {
                return true;
            }
        }

        return false;
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
