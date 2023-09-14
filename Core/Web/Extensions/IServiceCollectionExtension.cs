using Microsoft.Extensions.DependencyInjection;
using ModularSystem.Core;
using System.Text.Json.Serialization;

namespace ModularSystem.Web;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to simplify the addition of controllers
/// with custom JSON converters.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds controllers to the services and configures them to use the provided JSON converters.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="converters">An IEnumerable of JSON converters to be used in serialization and deserialization.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
    public static IMvcBuilder AddControllersWithJsonConverters(this IServiceCollection services, IEnumerable<JsonConverter> converters)
    {
        // Add controllers and custom converters to MVC builder
        return services.AddControllers()
            .AddJsonConverters(converters);
    }

    /// <summary>
    /// Adds controllers to the services and configures JSON serialization options with globally-defined JSON converters
    /// from <see cref="JsonSerializerSingleton"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
    public static IMvcBuilder AddControllersWithGlobalJsonConverters(this IServiceCollection services)
    {
        // Add controllers and globally-defined converters to MVC builder
        return services.AddControllers()
            .AddGlobalJsonConverters();
    }
}
