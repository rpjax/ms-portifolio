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
internal interface IFactory<out T>
{
    /// <summary>
    /// Creates and returns an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T Create();
}
