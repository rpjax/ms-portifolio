using Microsoft.Extensions.DependencyInjection;
using ModularSystem.Core;
using System.Text.Json.Serialization;

namespace ModularSystem.Web;

/// <summary>
/// Provides extension methods for <see cref="IMvcBuilder"/> to simplify the addition of JSON converters.
/// </summary>
public static class IMvcBuilderExtensions
{
    public static IMvcBuilder AddJsonConverter(this IMvcBuilder builder, JsonConverter converter)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(converter);
        });
    }

    /// <summary>
    /// Adds a collection of <see cref="JsonConverter"/> to the MVC builder's JSON serialization options.
    /// </summary>
    /// <param name="builder">The MVC builder to which the converters will be added.</param>
    /// <param name="converters">A collection of JSON converters to add.</param>
    /// <returns>An instance of <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
    public static IMvcBuilder AddJsonConverters(this IMvcBuilder builder, IEnumerable<JsonConverter> converters)
    {
        return builder.AddJsonOptions(options =>
        {
            foreach (var converter in converters)
            {
                options.JsonSerializerOptions.Converters.Add(converter);
            }
        });
    }

    /// <summary>
    /// Adds globally-defined JSON converters from <see cref="JsonSerializerSingleton"/> to the MVC builder's JSON serialization options.
    /// </summary>
    /// <param name="builder">The MVC builder to which the converters will be added.</param>
    /// <returns>An instance of <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
    public static IMvcBuilder AddGlobalJsonConverters(this IMvcBuilder builder)
    {
        return builder.AddJsonConverters(JsonSerializerSingleton.GetConverters());
    }
}
