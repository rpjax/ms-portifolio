using ModularSystem.Core;
using ModularSystem.Web.Expressions;

namespace ModularSystem.Web;

internal class WebInitializer : Initializer
{
    protected internal override Task InternalInitAsync(Options options)
    {
        JsonSerializerSingleton.TryAddConverter(new ExprJsonConverter());

        return base.InternalInitAsync(options);
    }
}
