using ModularSystem.Core.Pipes;

namespace ModularSystem.Core;

public interface IPipeline<TIn, TOut>
{
    IPipeline<TIn, TOut>? Next { get; }

    IPipeline<TIn, TOut> Append(IPipeline<TIn, TOut> next);
    IPipeline<TIn, TOut> Append(Func<TIn, TOut> lambda);
    IPipeline<TIn, TOut> Append(Func<TIn, Task<TOut>> asyncLambda);
    IPipeline<TIn, TOut> Append<T>() where T : IPipeline<TIn, TOut>, new();

    IPipeline<TIn, TOut> Push(IPipeline<TIn, TOut> next);
    IPipeline<TIn, TOut> Push(Func<TIn, TOut> lambda);
    IPipeline<TIn, TOut> Push(Func<TIn, Task<TOut>> asyncLambda);
    IPipeline<TIn, TOut> Push<T>() where T : IPipeline<TIn, TOut>, new();

    IPipeline<TIn, TOut> SetNext(IPipeline<TIn, TOut> next);

    Task<TOut> RunAsync(TIn data);
}

public abstract class Pipeline<TIn, TOut> : IPipeline<TIn, TOut>
{
    public IPipeline<TIn, TOut>? Next { get; protected set; }

    protected Pipeline(IPipeline<TIn, TOut>? next = null)
    {
        Next = next;
    }

    public IPipeline<TIn, TOut> Append(IPipeline<TIn, TOut> next)
    {
        if (Next == null)
        {
            return SetNext(next);
        }
        else
        {
            var _next = Next;

            while (_next.Next != null)
            {
                _next = _next.Next;
            }

            _next.Append(next);
            return this;
        }
    }

    public IPipeline<TIn, TOut> Append(Func<TIn, TOut> lambda)
    {
        Append(new LambdaPipe<TIn, TOut>(lambda));
        return this;
    }

    public IPipeline<TIn, TOut> Append(Func<TIn, Task<TOut>> asyncLambda)
    {
        Append(new LambdaPipe<TIn, TOut>(asyncLambda));
        return this;
    }

    public IPipeline<TIn, TOut> Append<T>() where T : IPipeline<TIn, TOut>, new()
    {
        if (Next == null)
        {
            return SetNext(new T());
        }
        else
        {
            var _next = Next;

            while (_next.Next != null)
            {
                _next = _next.Next;
            }

            _next.Append<T>();
            return this;
        }
    }

    public IPipeline<TIn, TOut> Push(IPipeline<TIn, TOut> next)
    {
        return next.SetNext(this);
    }

    public IPipeline<TIn, TOut> Push(Func<TIn, TOut> lambda)
    {
        Push(new LambdaPipe<TIn, TOut>(lambda));
        return this;
    }

    public IPipeline<TIn, TOut> Push(Func<TIn, Task<TOut>> asyncLambda)
    {
        Push(new LambdaPipe<TIn, TOut>(asyncLambda));
        return this;
    }

    public IPipeline<TIn, TOut> Push<T>() where T : IPipeline<TIn, TOut>, new()
    {
        return new T().SetNext(this);
    }

    public IPipeline<TIn, TOut> SetNext(IPipeline<TIn, TOut> next)
    {
        Next = next;
        return this;
    }

    public virtual Task<TOut> RunAsync(TIn data)
    {
        if (Next == null)
        {
            throw new InvalidOperationException("IPipeline reached the end and no hander was found.");
        }

        return Next.RunAsync(data);
    }
}