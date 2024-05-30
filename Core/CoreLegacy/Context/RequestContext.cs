using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Core;

public class RequestContext
{
    public string? UniqueKey { get; set; } = null;
    public IIdentity? Identity { get; set; } = null;

    public static RequestContext From(IIdentity credential)
    {
        return new RequestContext()
        {
            Identity = credential
        };
    }
}