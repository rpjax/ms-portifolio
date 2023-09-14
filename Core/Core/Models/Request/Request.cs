namespace ModularSystem.Core;

public interface IRequest
{
    RequestContext Context { get; }
    string UserKey();
}

public class Request : IRequest
{
    public RequestContext Context { get; protected set; }

    public Request(RequestContext context)
    {
        Context = context;
    }

    public virtual string UserKey()
    {
        return Context.Identity.UniqueIdentifier;
    }
}

public class Request<T> : Request
{
    public T Data { get; private set; }

    public Request(RequestContext context, T data) : base(context)
    {
        Data = data;
    }
}