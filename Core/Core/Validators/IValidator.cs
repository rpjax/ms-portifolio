namespace ModularSystem.Core;

/// <summary>
/// Defines a contract for asynchronous validation of objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the specified instance asynchronously.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="T"/> to validate.</param>
    /// <returns>
    /// A task that represents the asynchronous validation operation. The task result contains
    /// an exception if the validation fails; otherwise, null.
    /// </returns>
    Task<Exception?> ValidateAsync(T instance);
}

/// <summary>
/// Defines a contract for synchronous validation of objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public interface ISyncValidator<T>
{
    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="T"/> to validate.</param>
    void Validate(T instance);
}

/// <summary>
/// Defines a contract for asynchronous validation of objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public interface IAsyncValidator<T>
{
    /// <summary>
    /// Validates the specified instance asynchronously.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="T"/> to validate.</param>
    /// <returns>
    /// A task that represents the asynchronous validation operation.
    /// </returns>
    Task ValidateAsync(T instance);
}


/// <summary>
/// Represents a validator for type <typeparamref name="T"/> that always considers the instance as valid.
/// This is a "no-op" or "pass-through" validator that doesn't perform any validation checks.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public class EmptyValidator<T> : IValidator<T>
{
    /// <summary>
    /// Validates the specified instance but always returns a successful validation.
    /// </summary>
    /// <param name="instance">The instance of type <typeparamref name="T"/> to validate.</param>
    /// <returns>
    /// A task that represents the asynchronous validation operation. Since this is an empty validator,
    /// the task result always contains null, indicating a successful validation.
    /// </returns>
    public Task<Exception?> ValidateAsync(T instance)
    {
        return Task.FromResult<Exception?>(null);
    }
}
