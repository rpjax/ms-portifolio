﻿using Microsoft.Extensions.DependencyInjection;
using ModularSystem.Core;

namespace ModularSystem.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddIamService(this IServiceCollection services, IIamService iamSystem)
    {
        DependencyContainer.TryRegister(iamSystem);

        return services.AddSingleton(iamSystem);
    }
}
