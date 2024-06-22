using ModularSystem.Core;

namespace Webql.Core;

public class WebqlCompilationContext
{
    public WebqlCompilerSettings Settings { get; }

    private List<Error> Errors { get; } 

    public WebqlCompilationContext(WebqlCompilerSettings settings)
    {
        Settings = settings;
        Errors = new List<Error>();
    }

    public Type QueryableType => Settings.QueryableType;
    public Type EntityType => Settings.EntityType;
    public Type EntityQueryableType => QueryableType.MakeGenericType(EntityType);

}
