﻿using ModularSystem.Core.Linq;

namespace Webql.Core;

public class WebqlCompilerSettings
{
    public static Type DefaultQueryableType { get; } = typeof(IQueryable<>);
    public static Type DefaultAsyncQueryableType { get; } = typeof(IAsyncQueryable<>);

    public Type QueryableType { get; } 
    public Type ElementType { get; }

    public WebqlCompilerSettings(Type queryableType, Type entityType)
    {
        QueryableType = queryableType;
        ElementType = entityType;
    }
}
