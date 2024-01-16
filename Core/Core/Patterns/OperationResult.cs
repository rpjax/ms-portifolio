namespace ModularSystem.Core;

/// <summary>
/// Interface defining a component that can produce errors.
/// </summary>
public interface IErrorProducer
{
    /// <summary>
    /// Gets a collection of validation errors.
    /// </summary>
    IEnumerable<ValidationError> Errors { get; }
}

/// <summary>
/// Interface representing the result of an operation, including any validation results and errors.
/// </summary>
public interface IOperationResult : IErrorProducer
{
    /// <summary>
    /// Gets the validation result of the operation, if available.
    /// </summary>
    ValidationResult? ValidationResult { get; }
}

/// <summary>
/// Generic interface extending IOperationResult to include operation data.
/// </summary>
/// <typeparam name="T">The type of data returned by the operation.</typeparam>
public interface IOperationResult<T> : IOperationResult
{
    /// <summary>
    /// Gets the data produced by the operation.
    /// </summary>
    T Data { get; }
}

/// <summary>
/// Represents the result of an operation, including validation results and a flag indicating success or failure.
/// </summary>
public class OperationResult : IOperationResult
{
    /// <inheritdoc />
    public ValidationResult? ValidationResult { get; init; }

    /// <inheritdoc />
    public IEnumerable<ValidationError> Errors { get => GetValidationErrors(); }

    /// <summary>
    /// Gets a value indicating whether the operation was successful (no errors).
    /// </summary>
    public bool IsSuccess { get => Errors.IsEmpty(); }

    /// <summary>
    /// Gets a value indicating whether the operation failed (has errors).
    /// </summary>
    public bool IsFailure { get => Errors.IsNotEmpty(); }

    /// <summary>
    /// Initializes a new instance of the OperationResult class, optionally with results from another operation.
    /// </summary>
    /// <param name="result">An optional IOperationResult to initialize the current instance with.</param>
    public OperationResult(IOperationResult? result = null)
    {
        ValidationResult = result?.ValidationResult;
    }

    /// <summary>
    /// Retrieves the collection of validation errors, if any.
    /// </summary>
    /// <returns>A collection of ValidationError objects.</returns>
    private IEnumerable<ValidationError> GetValidationErrors()
    {
        if (ValidationResult == null)
        {
            return Enumerable.Empty<ValidationError>();
        }

        return ValidationResult.Errors;
    }
}

/// <summary>
/// Represents the result of an operation, including validation results, a flag indicating success or failure, and operation data.
/// </summary>
/// <typeparam name="T">The type of data included in the operation result.</typeparam>
public class OperationResult<T> : OperationResult, IOperationResult<T?>
{
    /// <summary>
    /// Gets the data produced by the operation, if any.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with a specific type, optionally with results and data from another operation.
    /// </summary>
    /// <param name="operationResult">An optional IOperationResult of the same type to initialize the current instance with.</param>
    public OperationResult(IOperationResult<T>? operationResult = null) : base(operationResult)
    {
        Data = operationResult == null
            ? default
            : operationResult.Data;
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with a specific validation result.
    /// This constructor is typically used when the operation has failed and a ValidationResult is available.
    /// </summary>
    /// <param name="validationResult">The ValidationResult associated with the failed operation.</param>
    public OperationResult(ValidationResult validationResult)
    {
        ValidationResult = validationResult;
        Data = default;
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with operation data.
    /// This constructor is typically used when the operation is successful and data is available.
    /// </summary>
    /// <param name="data">The data produced by the successful operation.</param>
    public OperationResult(T? data)
    {
        ValidationResult = null;
        Data = data;
    }

}
