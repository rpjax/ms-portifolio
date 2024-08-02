using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Aidan.Core.Extensions;

/// <summary>
/// Provides extension methods for string manipulation.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is empty or null.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>true if the string is empty or null; otherwise, false.</returns>
    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// Determines whether the specified string is not empty and not null.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>true if the string is not empty and not null; otherwise, false.</returns>
    public static bool IsNotEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// Encodes the specified string to Base64 using the specified encoding.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="encoding">The encoding to use. If null, UTF-8 encoding is used.</param>
    /// <returns>The Base64 encoded string.</returns>
    public static string EncodeBase64(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var valueBytes = encoding.GetBytes(value);
        return Convert.ToBase64String(valueBytes);
    }

    /// <summary>
    /// Decodes the specified Base64 encoded string using the specified encoding.
    /// </summary>
    /// <param name="value">The Base64 encoded string to decode.</param>
    /// <param name="encoding">The encoding to use. If null, UTF-8 encoding is used.</param>
    /// <returns>The decoded string.</returns>
    public static string DecodeBase64(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var valueBytes = Convert.FromBase64String(value);
        return encoding.GetString(valueBytes);
    }

    /// <summary>
    /// Encodes the specified string to Base64 using the specified encoding.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="encoding">The encoding to use. If null, UTF-8 encoding is used.</param>
    /// <returns>The Base64 encoded string.</returns>
    public static string ToBase64(this string value, Encoding? encoding = null)
    {
        return value.EncodeBase64(encoding);
    }

    /// <summary>
    /// Converts the specified string to a byte array using the specified encoding.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="encoding">The encoding to use. If null, UTF-8 encoding is used.</param>
    /// <returns>The byte array representation of the string.</returns>
    public static byte[] ToBytes(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetBytes(value);
    }

    /// <summary>
    /// Determines whether the specified string can be deserialized as JSON of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the string as.</typeparam>
    /// <param name="value">The string to check.</param>
    /// <param name="serializerOptions">The JSON serializer options. If null, default options are used.</param>
    /// <returns>true if the string can be deserialized as JSON of the specified type; otherwise, false.</returns>
    public static bool IsDeserializableAsJson<T>(this string value, JsonSerializerOptions? serializerOptions = null)
    {
        try
        {
            serializerOptions ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            JsonSerializer.Deserialize<T>(value, serializerOptions);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Removes backslashes from the specified string.
    /// </summary>
    /// <param name="value">The string to normalize.</param>
    /// <returns>The normalized string.</returns>
    public static string JsonNormalize(this string value)
    {
        return value.Replace("\\", string.Empty);
    }

    /// <summary>
    /// Converts the specified string to a memory stream using the specified encoding.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="encoding">The encoding to use. If null, UTF-8 encoding is used.</param>
    /// <returns>The memory stream representation of the string.</returns>
    public static MemoryStream ToMemoryStream(this string str, Encoding? encoding = null)
    {
        return new MemoryStream(str.ToBytes(encoding));
    }

    /// <summary>
    /// Converts the provided string to the camel case pattern.
    /// </summary>
    /// <remarks>
    /// Example: "HelloWorld" becomes "helloWorld".
    /// </remarks>
    /// <param name="value">The string to be converted to camel case.</param>
    /// <returns>The camel-cased representation of the provided string.</returns>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // If the value already starts with a lowercase character, return as is.
        if (char.IsLower(value[0]))
        {
            return value;
        }

        // Convert the first character to lowercase.
        string camelCase = char.ToLower(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

        return camelCase;
    }

    /// <summary>
    /// Converts the provided string to the Pascal case pattern.
    /// </summary>
    /// <remarks>
    /// Example: "helloWorld" becomes "HelloWorld".
    /// </remarks>
    /// <param name="value">The string to be converted to Pascal case.</param>
    /// <returns>The Pascal-cased representation of the provided string.</returns>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // If the value already starts with an uppercase character, return as is.
        if (char.IsUpper(value[0]))
        {
            return value;
        }

        // Convert the first character to uppercase.
        string pascalCase = char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

        return pascalCase;
    }

    /// <summary>
    /// Converts the provided string to the snake case pattern.
    /// </summary>
    /// <remarks>
    /// Example: "HelloWorld" becomes "hello_world".
    /// </remarks>
    /// <param name="value">The string to be converted to snake case.</param>
    /// <returns>The snake-cased representation of the provided string.</returns>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    sb.Append('_');
                }
                sb.Append(char.ToLower(c, CultureInfo.CurrentCulture));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the provided string to the kebab case pattern.
    /// </summary>
    /// <remarks>
    /// Example: "HelloWorld" becomes "hello-world".
    /// </remarks>
    /// <param name="value">The string to be converted to kebab case.</param>
    /// <returns>The kebab-cased representation of the provided string.</returns>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    sb.Append('-');
                }
                sb.Append(char.ToLower(c, CultureInfo.CurrentCulture));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the provided string to title case.
    /// </summary>
    /// <remarks>
    /// Example: "hello world" becomes "Hello World".
    /// </remarks>
    /// <param name="value">The string to be converted to title case.</param>
    /// <returns>The title-cased representation of the provided string.</returns>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }

    /// <summary>
    /// Converts the provided string to sentence case.
    /// </summary>
    /// <remarks>
    /// Example: "hello world" becomes "Hello world".
    /// </remarks>
    /// <param name="value">The string to be converted to sentence case.</param>
    /// <returns>The sentence-cased representation of the provided string.</returns>
    public static string ToSentenceCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);
    }

    /// <summary>
    /// Converts the provided string to lowercase, with the first character in lowercase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The string with the first character in lowercase.</returns>
    public static string ToLowerFirst(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToLower(value[0], CultureInfo.CurrentCulture) + value.Substring(1);
    }

    /// <summary>
    /// Converts the provided string to uppercase, with the first character in uppercase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The string with the first character in uppercase.</returns>
    /// public static string ToUpperFirst(this string value)
    public static string ToUpperFirst(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);
    }
}
