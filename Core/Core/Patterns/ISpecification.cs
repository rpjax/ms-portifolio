namespace ModularSystem.Core;

/// <summary>
/// Defines a contract for a specification pattern. <br/>
/// A specification pattern is used to encapsulate business rules that can be re-used across different parts of an application. <br/>
/// It represents a single business rule
/// that determines if a particular criteria is satisfied by a given instance.
/// </summary>
/// <typeparam name="T">The type of object to which the specification is applied.</typeparam>
public interface ISpecification<in T>
{
    /// <summary>
    /// Determines whether a specific instance satisfies the criteria defined by the specification.
    /// </summary>
    /// <param name="instance">The instance to be checked against the specification.</param>
    /// <returns><c>true</c> if the instance satisfies the specification; otherwise, <c>false</c>.</returns>
    bool IsSatisfiedBy(T instance);
}

/// <summary>
/// Defines a contract for an asynchronous specification pattern. <br/>
/// Similar to <see cref="ISpecification{T}"/>, but allows for the criteria check to be performed asynchronously. <br/>
/// This is useful in scenarios where the satisfaction of the criteria might depend on I/O operations <br/>
/// or other asynchronous operations.
/// </summary>
/// <typeparam name="T">The type of object to which the specification is applied.</typeparam>
public interface IAsyncSpecification<in T>
{
    /// <summary>
    /// Asynchronously determines whether a specific instance satisfies the criteria defined by the specification.
    /// </summary>
    /// <param name="instance">The instance to be checked against the specification.</param>
    /// <returns>A task representing the asynchronous operation. The task result is <c>true</c> if the instance
    /// satisfies the specification; otherwise, <c>false</c>.</returns>
    Task<bool> IsSatisfiedByAsync(T instance);
}
