using ModularSystem.Core.Extensions;

namespace ModularSystem.Core.Patterns;

/// <summary>
/// Represents the outcome of a non-atomic operation, indicating that the operation might <br/>
/// have succeeded, failed, or succeeded in part, containing any related errors.
/// </summary>
public class NonAtomicOperationResult : OperationResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was partially successful. <br/>
    /// This means the operation succeeded, but there were some errors.
    /// </summary>
    /// <value>
    /// True if the operation was successful and there are errors; otherwise, false.
    /// </value>
    public bool IsPartialSuccess => IsSuccess && Errors.IsNotEmpty();

    /// <summary>
    /// Initializes a new instance of the NonAtomicOperationResult class with a success status
    /// and a collection of errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was overall successful.</param>
    /// <param name="errors">A collection of errors encountered during the operation.</param>
    public NonAtomicOperationResult(bool isSuccess, IEnumerable<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Initializes a new instance with a given success status and an array of errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was overall successful.</param>
    /// <param name="errors">An array of errors encountered during the operation.</param>
    public NonAtomicOperationResult(bool isSuccess, params Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
}

/// <summary>
/// Represents the outcome of a non-atomic operation that includes operation-specific data, <br/>
/// indicating that the operation might have succeeded, failed, or succeeded in part.
/// </summary>
/// <typeparam name="T">The type of data included in the operation result.</typeparam>
public class NonAtomicOperationResult<T> : OperationResult<T>
{
    /// <summary>
    /// Initializes a new instance of the NonAtomicOperationResult class with a success status,
    /// operation-specific data, and a collection of errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was overall successful.</param>
    /// <param name="data">The data produced by the operation, if any.</param>
    /// <param name="errors">A collection of errors encountered during the operation.</param>
    public NonAtomicOperationResult(bool isSuccess, T? data, IEnumerable<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// Initializes a new instance with a given success status, operation-specific data, and an array of errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was overall successful.</param>
    /// <param name="data">The data produced by the operation, if any.</param>
    /// <param name="errors">An array of errors encountered during the operation.</param>
    public NonAtomicOperationResult(bool isSuccess, T? data, params Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
}
