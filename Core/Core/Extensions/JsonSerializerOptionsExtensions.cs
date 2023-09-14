using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions AddConverter(this JsonSerializerOptions options, JsonConverter converter)
    {
        options.Converters.Add(converter);
        return options;
    }
}