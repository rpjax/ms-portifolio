using System.Collections;

namespace ModularSystem.Core;

/// <summary>
/// Represents a collection of <see cref="Error"/> objects and provides methods to iterate over them.
/// </summary>
public class ValidationResult : IEnumerable<Error>
{
    /// <summary>
    /// Gets a value indicating whether the validation result contains no errors.
    /// </summary>
    public bool IsEmpty => Errors.Count <= 0;

    /// <summary>
    /// Gets a value indicating whether the validation result contains errors.
    /// </summary>
    public bool IsNotEmpty => !IsEmpty;

    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<Error> Errors { get; set; } = new();

    /// <summary>
    /// Returns an enumerator that iterates through the collection of validation errors.
    /// </summary>
    /// <returns>An enumerator for the validation errors.</returns>
    public IEnumerator<Error> GetEnumerator()
    {
        return Errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns a string representation of all validation errors, separated by new lines.
    /// </summary>
    /// <returns>A string containing all validation errors.</returns>
    public override string ToString()
    {
        return string.Join(";" + Environment.NewLine, Errors);
    }
}

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
    ValidationResult Validate(T instance);
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
    Task<ValidationResult> ValidateAsync(T instance);
}

/// <summary>
/// Represents the base class for validators, providing common functionalities like error collection and aggregation.
/// </summary>
public abstract class ValidatorBase
{
    /// <summary>
    /// Gets a value indicating whether the validation process should stop after the first error is found.
    /// </summary>
    public bool EnableShortCircuit { get; } = false;

    /// <summary>
    /// Protected property to access the cumulative validation results.
    /// </summary>
    protected ValidationResult Result { get; } = new();

    /// <summary>
    /// Adds one or more <see cref="Error"/> objects to the validation result.
    /// </summary>
    /// <param name="errors">The array of ValidationError objects to add.</param>
    protected void AddErrors(params Error[] errors)
    {
        Result.Errors.AddRange(errors);
    }

    /// <summary>
    /// Combines the current validation result with another, optionally prefixing errors with a source identifier.
    /// </summary>
    /// <param name="result">The ValidationResult to combine with the current result.</param>
    /// <param name="source">Optional source identifier to prefix each error message.</param>
    /// <param name="separator">The separator used between source and error message.</param>
    protected void Combine(OperationResult result, string? source = null, string? separator = ".")
    {
        var errors = result.Errors
            .Transform(x => x.AppendSource(source, separator))
            .ToArray();

        AddErrors(errors);
    }

    /// <summary>
    /// Asynchronously combines the current validation result with the result of a provided validation task.
    /// </summary>
    /// <param name="validationTask">The validation task whose result is to be combined.</param>
    /// <param name="source">Optional source identifier to prefix each error message.</param>
    /// <param name="separator">The separator used between source and error message.</param>
    protected async Task CombineAsync(Task<OperationResult> validationTask, string? source = null, string? separator = ".")
    {
        Combine(await validationTask, source, separator);
    }
}

/// <summary>
/// Provides an abstract base for asynchronous validators of objects of type <typeparamref name="T"/>. <br/>
/// This class extends <see cref="ValidatorBase"/> to include asynchronous validation logic.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public abstract class AsyncValidator<T> : ValidatorBase, IAsyncValidator<T>
{
    /// <summary>
    /// Asynchronously validates an instance of type <typeparamref name="T"/>. <br/>
    /// This method iterates through potential errors, adding them to the validation result. <br/>
    /// If <see cref="ValidatorBase.EnableShortCircuit"/> is true, the validation stops at the first error.
    /// </summary>
    /// <param name="input">The instance of type <typeparamref name="T"/> to be validated.</param>
    /// <returns>
    /// A task representing the asynchronous validation operation, yielding a <see cref="OperationResult"/> 
    /// that contains all identified validation errors.
    /// </returns>
    public virtual async Task<ValidationResult> ValidateAsync(T input)
    {
        await using var errorsEnumerator = EnumerateErrorsAsync(input).GetAsyncEnumerator();

        while (await errorsEnumerator.MoveNextAsync())
        {
            AddErrors(errorsEnumerator.Current);

            if (EnableShortCircuit)
            {
                break;
            }
        }

        return Result;
    }

    /// <summary>
    /// When overridden in a derived class, provides an asynchronous enumerable of <see cref="Error"/> <br/>
    /// for a given input. This method should be implemented to define the validation logic specific to the type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="input">The instance of type <typeparamref name="T"/> to validate.</param>
    /// <returns>An asynchronous enumerable of <see cref="Error"/> objects, representing the validation errors.</returns>
    protected abstract IAsyncEnumerable<Error> EnumerateErrorsAsync(T input);
}

/// <summary>
/// Represents a validator for type <typeparamref name="T"/> that always considers the instance as valid.
/// This is a "no-op" or "pass-through" validator that doesn't perform any validation checks.
/// </summary>
/// <typeparam name="T">The type of the object to be validated.</typeparam>
public class EmptyValidator<T> : ValidatorBase, IValidator<T>
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
