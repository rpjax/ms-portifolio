namespace ModularSystem.Core;

/// <summary>
/// Defines a strategy pattern interface for executing an operation on a given state.
/// </summary>
/// <remarks>
/// The strategy pattern is used to define a family of algorithms, encapsulate each one,
/// and make them interchangeable. This interface allows for the implementation of different
/// strategies that operate on a given state, enabling the dynamic changing of algorithms or
/// behaviors within an application depending on the current context or requirements.
/// </remarks>
/// <typeparam name="TState">The type of the state on which the strategy operates.</typeparam>
public interface IStrategy<TState>
{
    /// <summary>
    /// Executes the strategy on the provided state.
    /// </summary>
    /// <param name="state">The state to be processed or altered by the strategy.</param>
    void Execute(TState state);
}

/// <summary>
/// Defines a synchronous strategy pattern interface, representing a single algorithm or action <br/>
/// that can be applied to an input of type <typeparamref name="TState"/> to produce an output of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TState">The type of input the strategy operates on.</typeparam>
/// <typeparam name="TResult">The type of output the strategy produces.</typeparam>
public interface IStrategy<TState, TResult>
{
    /// <summary>
    /// Executes the strategy on the provided input.
    /// </summary>
    /// <param name="state">The input on which the strategy is applied.</param>
    /// <returns>The result of the strategy execution or null if the strategy produces no result for the given input.</returns>
    TResult Execute(TState state);
}

/// <summary>
/// Defines an asynchronous strategy pattern interface, representing a single algorithm or action <br/>
/// that can be applied to an input of type <typeparamref name="TState"/> to produce an output of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TState">The type of input the strategy operates on.</typeparam>
/// <typeparam name="TResult">The type of output the strategy produces.</typeparam>
public interface IAsyncStrategy<TState, TResult>
{
    /// <summary>
    /// Asynchronously executes the strategy on the provided input.
    /// </summary>
    /// <param name="state">The input on which the strategy is applied.</param>
    /// <returns>
    /// A task representing the asynchronous operation which, when completed, yields the result of the strategy execution 
    /// or null if the strategy produces no result for the given input.
    /// </returns>
    Task<TResult> ExecuteAsync(TState state);
}

public sealed class LambdaStrategy<TState> : IStrategy<TState>
{
    private readonly Action<TState> _action;

    public LambdaStrategy(Action<TState> action)
    {
        _action = action;
    }

    public void Execute(TState state)
    {
        _action.Invoke(state);
    }
}
