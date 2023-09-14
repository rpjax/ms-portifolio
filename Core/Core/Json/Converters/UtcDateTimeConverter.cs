using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

internal class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (value == null)
        {
            throw new JsonException("Invalid DateTime value. Value cannot be null or empty.");
        }

        if (!DateTime.TryParse(value, CultureProvider.CultureInfo, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result))
        {
            throw new JsonException($"Invalid DateTime format. Value: {value}");
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
    }
}