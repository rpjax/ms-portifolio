using ModularSystem.Core;
using ModularSystem.Web.Expressions;

namespace ModularSystem.Web;

[Obsolete("TO BE DELETED")]
internal class WebInitializer : Initializer
{
    protected override Task OnInitAsync(Options options)
    {
        JsonSerializerSingleton.TryAddConverter(new ExprJsonConverter());
        return base.OnInitAsync(options);
    }
}
