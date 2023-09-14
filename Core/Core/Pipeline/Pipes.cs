using ModularSystem.Core.Security;

namespace ModularSystem.Core.Pipes;

public class EmptyPipe<TIn, TOut> : Pipeline<TIn, TOut>
{

}

public class LambdaPipe<TIn, TOut> : Pipeline<TIn, TOut>
{
    Func<TIn, Task<TOut>> lambda;


    public LambdaPipe(Func<TIn, TOut> lambda)
    {
        this.lambda = x => Task.FromResult(lambda(x));
    }

    public LambdaPipe(Func<TIn, Task<TOut>> lambda) : base(null)
    {
        this.lambda = lambda;
    }

    public override Task<TOut> RunAsync(TIn data)
    {
        return lambda(data);
    }
}

public class AuthorizationPipe<TIn, TOut> : Pipeline<TIn, TOut>
{
    IResourcePolicy ResourcePolicy;
    IIdentity Identity;

    public AuthorizationPipe(IResourcePolicy resourcePolicy, IIdentity identity) : base(null)
    {
        ResourcePolicy = resourcePolicy;
        Identity = identity;
    }

    public AuthorizationPipe(IResourcePolicy resourcePolicy, IIdentity identity, IPipeline<TIn, TOut> next) : base(next)
    {
        ResourcePolicy = resourcePolicy;
        Identity = identity;
    }

    public override async Task<TOut> RunAsync(TIn request)
    {
        await ResourcePolicy.AuthorizeAsync(Identity);
        return await base.RunAsync(request);
    }
}