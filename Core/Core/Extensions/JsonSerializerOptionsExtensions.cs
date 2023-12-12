using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions CopyWith(this JsonSerializerOptions self, JsonSerializerOptions? options = null)
    {
        var copy = new JsonSerializerOptions();

        foreach (var converter in self.Converters)
        {
            if (!copy.Converters.Contains(converter))
            {
                copy.Converters.Add(converter);
            }
        }

        if (options == null)
        {
            return copy;
        }

        foreach (var converter in options.Converters)
        {
            if (!copy.Converters.Contains(converter))
            {
                copy.Converters.Add(converter);
            }
        }

        return copy;
    }

    public static JsonSerializerOptions AddConverter(this JsonSerializerOptions options, JsonConverter converter)
    {
        if (!options.Converters.Contains(converter))
        {
            options.Converters.Add(converter);
        }

        return options;
    }

    public static JsonSerializerOptions AddConverter<T>(this JsonSerializerOptions options) where T : JsonConverter, new()
    {
        return AddConverter(options, new T());
    }

    public static JsonSerializerOptions AddConverters(this JsonSerializerOptions self, JsonSerializerOptions? options)
    {
        if (options == null)
        {
            return self;
        }

        foreach (var converter in options.Converters)
        {
            if (!self.Converters.Contains(converter))
            {
                self.Converters.Add(converter);
            }
        }

        return self;
    }
}
