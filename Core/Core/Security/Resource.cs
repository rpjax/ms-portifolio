namespace ModularSystem.Core.Security;

public interface IResource
{
    string Service { get; }
    string Name { get; }
    string[] Actions { get; }

    IdentityPermission[] GetPermissions();
    IResourcePolicy GetResourcePolicy();
    IResourcePolicy GetResourcePolicy(string action);
}

public class Resource : IResource
{
    public string Service { get; set; }
    public string Name { get; set; }
    public string[] Actions { get => _actions.ToArray(); }

    List<string> _actions { get; }

    public Resource(string service, string name)
    {
        Service = service;
        Name = name;
        _actions = new();
    }

    public Resource(string service, string name, string[] actions)
    {
        Service = service;
        Name = name;
        _actions = new(actions);
    }

    public IdentityPermission[] GetPermissions()
    {
        var permissions = new List<IdentityPermission>(Actions.Length);

        foreach (var action in Actions)
        {
            permissions.Add(new IdentityPermission(Service, Name, action));
        }

        return permissions.ToArray();
    }

    public IResourcePolicy GetResourcePolicy()
    {
        return new ResourcePolicy()
            .SetRequiredPermissions(GetPermissions());
    }

    public IResourcePolicy GetResourcePolicy(string action)
    {
        return new ResourcePolicy()
            .SetRequiredPermission(new IdentityPermission(Service, Name, action));
    }

    public Resource SetRequiredPermission(string action)
    {
        if (!_actions.Contains(action))
        {
            _actions.Add(action);
        }

        return this;
    }

    public Resource SetRequiredPermissions(IEnumerable<string> actions)
    {
        foreach (var action in actions)
        {
            SetRequiredPermission(action);
        }

        return this;
    }
}