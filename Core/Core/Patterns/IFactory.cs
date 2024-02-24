namespace ModularSystem.Core;

/// <summary>
/// Defines a factory interface responsible for creating instances of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object that the factory creates. This interface supports covariance for the type parameter.</typeparam>
/// <remarks>
/// The <c>out</c> keyword on the type parameter <typeparamref name="T"/> indicates covariance, 
/// which means that for a given interface <see cref="IFactory{T}"/>, you can assign its instance 
/// to a variable of a derived type as long as it inherits or implements <typeparamref name="T"/>. 
/// Covariance allows for more flexibility when working with derived types.
/// </remarks>
public interface IFactory<out T>
{
    /// <summary>
    /// Creates and returns an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T Create();
}

/// <summary>
/// Defines an asynchronous factory interface responsible for creating instances of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object that the factory creates asynchronously.</typeparam>
/// <remarks>
/// This interface is tailored for scenarios where object creation might involve asynchronous operations, <br/>
/// such as fetching initial data from a remote source, asynchronous computations, or any other asynchronous <br/>
/// initializations that should be done during the object's instantiation process.
/// </remarks>
public interface IAsyncFactory<T>
{
    /// <summary>
    /// Asynchronously creates and returns an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing an instance of type <typeparamref name="T"/> upon completion.</returns>
    Task<T> CreateAsync();
}

public class LambdaFactory<T> : IFactory<T>
{
    Func<T> _lambda;

    public LambdaFactory(Func<T> lambda)
    {
        _lambda = lambda;
    }

    public T Create()
    {
        return _lambda.Invoke();
    }
}