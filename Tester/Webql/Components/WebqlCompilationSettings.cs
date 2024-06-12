using ModularSystem.Core.Linq;

namespace Webql.Components;

public class WebqlCompilationSettings
{
    public Type QueryableType { get; set; } = typeof(IAsyncQueryable<>);
}

public class WebqlCompilationContext
{
    public WebqlCompilationSettings Settings { get; }

    public WebqlCompilationContext(WebqlCompilationSettings settings)
    {
        Settings = settings;
    }
}
