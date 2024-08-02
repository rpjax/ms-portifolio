using Microsoft.Extensions.DependencyInjection;
using Aidan.Core;

namespace Aidan.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddIamService(this IServiceCollection services, IAccessManagementService iamSystem)
    {
        DependencyContainer.TryRegister(iamSystem);

        return services.AddSingleton(iamSystem);
    }
}
