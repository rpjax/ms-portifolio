using ModularSystem.Core;
using System.Reflection;
using System.Text;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a serializable representation of <see cref="ConstructorInfo"/>. Designed for facilitating<br/>
/// serialization into string-based formats such as JSON, XML, and others.
/// </summary>
public class SerializableConstructorInfo
{
    /// <summary>
    /// Gets or sets the type in which the constructor is declared.
    /// </summary>
    public SerializableType? DeclaringType { get; set; }

    /// <summary>
    /// Gets or sets the parameters' types of the constructor.
    /// </summary>
    public SerializableType[] Parameters { get; set; } = Array.Empty<SerializableType>();

    /// <summary>
    /// Generates a string representation of the constructor's signature.
    /// </summary>
    /// <returns>A string representing the constructor's signature.</returns>
    public string GetSignatureString()
    {
        var strBuilder = new StringBuilder();
        var parameters = Parameters.Transform(type
            => {
                var name = type.FullName ?? type.Name;
                return $"{name} {name?.ToCamelCase()}";
            }
        ).ToArray();

        strBuilder.Append(DeclaringType?.GetFullName());
        strBuilder.Append('(');
        strBuilder.Append(string.Join(", ", parameters));
        strBuilder.Append(')');

        return strBuilder.ToString();
    }
}
