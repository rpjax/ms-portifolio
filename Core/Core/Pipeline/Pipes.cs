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
