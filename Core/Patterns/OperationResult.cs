using ModularSystem.Core.Extensions;
using System.Text.Json.Serialization;

namespace ModularSystem.Core.Patterns;

/// <summary>
/// Interface representing the result of an operation, including any validation results and errors.
/// It defines the structure for operation results across the application.
/// </summary>
public interface IOperationResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a collection of errors encountered during the operation.
    /// </summary>
    IReadOnlyList<Error> Errors { get; }
}

/// <summary>
/// Generic interface extending IOperationResult to include operation data.
/// This interface allows for a strongly-typed data result along with the operation status.
/// </summary>
/// <typeparam name="T">The type of data returned by the operation.</typeparam>
public interface IOperationResult<T> : IOperationResult
{
    /// <summary>
    /// Gets the data produced by the operation, if any.
    /// </summary>
    T Data { get; }
}

/// <summary>
/// Represents the outcome of an atomic operation with an absolute success or failure. <br/>
/// Use <see cref="NonAtomicOperationResult"/> for operations with partial successes or failures.
/// </summary>
public class OperationResult : IOperationResult
{
    /// <summary>
    /// Provides a pre-defined instance representing a successful operation. <br/>
    /// </summary>
    public static readonly OperationResult Success = new();

    /// <summary>
    /// Indicates the success status of the operation.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// A list of encountered errors during the operation.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; init; } 

    /// <summary>
    /// Gets a value indicating whether the operation resulted in errors.
    /// </summary>
    [JsonIgnore]
    public bool ContainsErrors { get => Errors.IsNotEmpty(); }

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    public OperationResult()
    {
        IsSuccess = true;
        Errors = Array.Empty<Error>();
    }
    
    /// <summary>
    /// Creates an operation result with the specified success status and optional errors in case of failure.
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="errors"></param>
    /// <exception cref="ArgumentException"></exception>
    public OperationResult(bool isSuccess, params Error[] errors)
    {
        if(isSuccess && errors.IsNotEmpty())
        {
            throw new ArgumentException("An operation cannot be successful and contain errors at the same time.");
        }

        IsSuccess = true;
        Errors = errors;
    }

    /// <summary>
    /// Creates a failed operation result with a specified error.
    /// </summary>
    /// <param name="errors"> The errors causing the failure.</param>
    public OperationResult(params Error[] errors)
    {
        IsSuccess = false;
        Errors = errors;
    }

    /// <summary>
    /// Creates a failed operation result with multiple errors.
    /// </summary>
    /// <param name="errors">A list of errors causing failure.</param>
    public OperationResult(IEnumerable<Error> errors)
    {
        IsSuccess = false;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    [JsonIgnore]
    public bool IsFailure { get => !IsSuccess; }

    /// <summary>
    /// Returns a string representation of all errors, separated by new lines.
    /// </summary>
    /// <returns>A string containing all validation errors.</returns>
    public override string ToString()
    {
        return string.Join("; " + Environment.NewLine, Errors);
    }

}

/// <summary>
/// Represents the outcome of an atomic operation, indicating an absolute success or failure, <br/>
/// and containing operation-specific data if the operation was successful. <br/>
/// For operations that can have partial successes or failures, consider using the 
/// <see cref="NonAtomicOperationResult{T}"/> class.
/// </summary>
/// <typeparam name="T">The type of data included in the operation result, if any.</typeparam>
public class OperationResult<T> : OperationResult, IOperationResult<T?>
{
    /// <summary>
    /// The data produced by the operation.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful operation result with optional data.
    /// </summary>
    /// <param name="data">The operation result data, if any.</param>
    public OperationResult(T? data = default)
    {
        IsSuccess = true;
        Data = data;
    }

    /// <summary>
    /// Creates a failed operation result with a specified error.
    /// </summary>
    /// <param name="error">The error causing the operation failure.</param>
    public OperationResult(Error error) : base(error)
    {

    }

    /// <summary>
    /// Creates a failed operation result with multiple errors.
    /// </summary>
    /// <param name="errors">The errors causing the operation failure.</param>
    public OperationResult(IEnumerable<Error> errors) : base(errors)
    {

    }

    /// <summary>
    /// Retrieves the data from a successful operation, ensuring the operation was successful before accessing the data. <br/>
    /// Throws an exception if the operation failed or data is null.
    /// </summary>
    /// <returns>The data produced by the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation was unsuccessful or data is null.</exception>
    public T GetData()
    {
        if (!IsSuccess)
        {
            throw new InvalidOperationException("Cannot retrieve data from an unsuccessful operation result. Check 'IsSuccess' before calling 'GetData'.");
        }

        if (Data == null)
        {
            throw new InvalidOperationException("The data is null. This operation result indicates success but contains no data.");
        }

        return Data;
    }


    /// <summary>
    /// Updates the operation result with new data and returns the instance.
    /// </summary>
    /// <param name="data">The new data for the operation result.</param>
    /// <returns>The updated OperationResult instance.</returns>
    public OperationResult<T> SetData(T? data)
    {
        Data = data;
        return this;
    }

}



