using System.Text;
using System.Text.Json;

namespace ModularSystem.Core;

public static class StringExtensions
{
    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNotEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static string EncodeBase64(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var valueBytes = encoding.GetBytes(value);
        return Convert.ToBase64String(valueBytes);
    }

    public static string DecodeBase64(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var valueBytes = Convert.FromBase64String(value);
        return encoding.GetString(valueBytes);
    }

    public static string ToBase64(this string value, Encoding? encoding = null)
    {
        return value.EncodeBase64(encoding);
    }

    public static byte[] ToBytes(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetBytes(value);
    }

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

    public static string JsonNormalize(this string value)
    {
        return value.Replace("\\", string.Empty);
    }

    public static MemoryStream ToMemoryStream(this string str, Encoding? encoding = null)
    {
        return new MemoryStream(str.ToBytes(encoding));
    }
}
