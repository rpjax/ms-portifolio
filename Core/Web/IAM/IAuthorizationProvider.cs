using ModularSystem.Core.Security;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Under development and should not be used yet.
/// </summary>
public interface IAuthorizationProvider
{
    IResourcePolicy GetResourcePolicy(string domain, string resource);
}

public class DefaultAuthorizationProvider : IAuthorizationProvider
{
    List<Resource> Resources { get; set; }

    public DefaultAuthorizationProvider(List<Resource>? resources = null)
    {
        Resources = resources ?? new(30);
    }

    public IResourcePolicy GetResourcePolicy(string domain, string resource)
    {
        throw new NotImplementedException();
    }
}