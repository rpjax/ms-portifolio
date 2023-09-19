namespace ModularSystem.Core;

/// <summary>
/// Defines a synchronous strategy pattern interface, representing a single algorithm or action <br/>
/// that can be applied to an input of type <typeparamref name="TIn"/> to produce an output of type <typeparamref name="TOut"/>.
/// </summary>
/// <typeparam name="TIn">The type of input the strategy operates on.</typeparam>
/// <typeparam name="TOut">The type of output the strategy produces.</typeparam>
public interface IStrategy<TIn, TOut>
{
    /// <summary>
    /// Executes the strategy on the provided input.
    /// </summary>
    /// <param name="input">The input on which the strategy is applied.</param>
    /// <returns>The result of the strategy execution or null if the strategy produces no result for the given input.</returns>
    TOut? Execute(TIn? input);
}

/// <summary>
/// Defines an asynchronous strategy pattern interface, representing a single algorithm or action <br/>
/// that can be applied to an input of type <typeparamref name="TIn"/> to produce an output of type <typeparamref name="TOut"/>.
/// </summary>
/// <typeparam name="TIn">The type of input the strategy operates on.</typeparam>
/// <typeparam name="TOut">The type of output the strategy produces.</typeparam>
public interface IAsyncStrategy<TIn, TOut>
{
    /// <summary>
    /// Asynchronously executes the strategy on the provided input.
    /// </summary>
    /// <param name="input">The input on which the strategy is applied.</param>
    /// <returns>
    /// A task representing the asynchronous operation which, when completed, yields the result of the strategy execution 
    /// or null if the strategy produces no result for the given input.
    /// </returns>
    Task<TOut?> ExecuteAsync(TIn? input);
}
