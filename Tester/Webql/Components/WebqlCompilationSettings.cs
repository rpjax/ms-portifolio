using ModularSystem.Core;
using ModularSystem.Core.Linq;

namespace Webql.Components;

public class WebqlCompilationSettings
{
    public static Type DefaultQueryableType { get; } = typeof(IQueryable<>);
    public static Type DefaultAsyncQueryableType { get; } = typeof(IAsyncQueryable<>);

    public Type QueryableType { get; } 
    public Type EntityType { get; }

    public WebqlCompilationSettings(Type queryableType, Type entityType)
    {
        QueryableType = queryableType;
        EntityType = entityType;
    }
}

public class WebqlCompilationContext
{
    public WebqlCompilationSettings Settings { get; }

    private List<Error> Errors { get; } 

    public WebqlCompilationContext(WebqlCompilationSettings settings)
    {
        Settings = settings;
        Errors = new List<Error>();
    }

    public Type QueryableType => Settings.QueryableType;
    public Type EntityType => Settings.EntityType;
    public Type EntityQueryableType => QueryableType.MakeGenericType(EntityType);

}
