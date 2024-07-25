namespace ModularSystem.Core;

/// <summary>
/// Provides an abstraction for obtaining repository instances. 
/// </summary>
public interface IRepositoryProvider
{
    /// <summary>
    /// Gets the repository for the specified type.
    /// This method is designed to abstract the process of obtaining a repository instance,
    /// <br/>
    /// ensuring that the consumer can access repositories without knowing about their concrete implementations.
    /// </summary>
    /// <typeparam name="T">The type of the repository. This type parameter should represent an interface that extends <see cref="IRepository{T}"/>.</typeparam>
    /// <returns>The repository instance for the specified type. The returned repository is ready to be used for data operations.</returns>
    IRepository<T> GetRepository<T>();
}
