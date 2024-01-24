namespace ModularSystem.Core;

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
    List<Error> Errors { get; }
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
/// Represents the outcome of an operation, indicating whether it was successful or not, and containing any related errors. <br/>
/// This class offers foundational support for error handling and operation status assessment.
/// </summary>
public class OperationResult : IOperationResult
{
    public static readonly OperationResult Success = new();

    /// <inheritdoc/>
    public bool IsSuccess { get; protected set; }

    /// <inheritdoc/>
    public List<Error> Errors { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure { get => !IsSuccess; }

    /// <summary>
    /// Initializes a new instance of the OperationResult class based on another operation result.
    /// </summary>
    /// <param name="result">An IOperationResult to initialize the current instance with.</param>
    public OperationResult(IOperationResult result)
    {
        IsSuccess = result.IsSuccess;
        Errors = new List<Error>(result.Errors);
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with specified success status and errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="errors">The errors encountered during the operation.</param>
    public OperationResult(bool isSuccess = true, params Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = new List<Error>();
        Errors.AddRange(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class, potentially with a set of errors. <br/>
    /// This constructor is used to create an operation result and can represent either a successful or a failed operation based on the provided errors.
    /// </summary>
    /// <param name="errors">An array of <see cref="Error"/> objects representing the errors encountered during the operation. If no errors are provided, the operation is considered successful by default.</param>
    /// <remarks>
    /// If errors are provided, the operation's success status is set to <c>false</c>, indicating a failed operation. <br/>
    /// If no errors are passed (an empty array), the constructor treats the operation as successful, setting the success status to <c>true</c>.
    /// </remarks>
    public OperationResult(params Error[] errors) : this(false, errors)
    {
        if (errors.Length == 0)
        {
            IsSuccess = true;
        }
    }

    /// <summary>
    /// Returns a string representation of all errors, separated by new lines.
    /// </summary>
    /// <returns>A string containing all validation errors.</returns>
    public override string ToString()
    {
        return string.Join("; " + Environment.NewLine, Errors);
    }

    /// <summary>
    /// Retrieves only the errors marked as public.
    /// </summary>
    /// <returns>A collection of public errors.</returns>
    public IEnumerable<Error> GetPublicErrors()
    {
        return Errors.Where(x => x.ContainsFlags(ErrorFlags.Public));
    }

    /// <summary>
    /// Adds errors to the operation result.
    /// </summary>
    /// <param name="errors">The errors to add to the operation result.</param>
    /// <returns>The current instance with the added errors.</returns>
    public OperationResult AddErrors(params Error[] errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    /// <summary>
    /// Removes errors from the collection that match any of the specified flags.
    /// </summary>
    /// <param name="flags">The flags to check against the errors.</param>
    public OperationResult RemoveErrorsWithFlags(params string[] flags)
    {
        Errors.RemoveAll(x => x.ContainsFlags(flags));
        return this;
    }

    /// <summary>
    /// Removes errors from the collection that do not match any of the specified flags.
    /// </summary>
    /// <param name="flags">The flags to check against the errors.</param>
    public OperationResult RemoveErrorsWithoutFlags(params string[] flags)
    {
        Errors.RemoveAll(x => !x.ContainsFlags(flags));
        return this;
    }
}

/// <summary>
/// Represents the outcome of an operation, including a flag indicating success or failure, and potentially containing operation-specific data. <br/>
/// This class extends OperationResult to encapsulate data resulting from the operation, making it suitable for operations that yield a result.
/// </summary>
/// <typeparam name="T">The type of data included in the operation result, if any.</typeparam>
public class OperationResult<T> : OperationResult, IOperationResult<T?>
{
    /// <summary>
    /// Gets the data produced by the operation, if any.
    /// </summary>
    public T? Data { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult{T}"/> class using an existing <see cref="IOperationResult"/>. <br/>
    /// This constructor is used to create a typed operation result from a non-typed operation result.
    /// </summary>
    /// <param name="operationResult">The non-typed operation result to initialize the current instance with.</param>
    /// <remarks>
    /// The success status and errors from the provided operation result are preserved, <br/>
    /// but the data is set to its default value as the type information is not available in the non-typed operation result.
    /// </remarks>
    public OperationResult(IOperationResult operationResult) : base(operationResult)
    {
        Data = default;
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with specific operation data, <br/>
    /// optionally with results and data from another operation.
    /// </summary>
    /// <param name="operationResult">An optional IOperationResult of the same type to initialize the current instance with.</param>
    public OperationResult(IOperationResult<T> operationResult) : base(operationResult)
    {
        Data = operationResult.Data;
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class with success status, operation data, and errors.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="data">The data produced by the operation.</param>
    /// <param name="errors">The errors encountered during the operation.</param>
    public OperationResult(bool isSuccess = true, T? data = default, params Error[] errors) : base(isSuccess, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class assuming a successful operation, with operation data and errors. <br/>
    /// This constructor is typically used when the operation is successful and data is available.
    /// </summary>
    /// <param name="data">The data produced by the successful operation.</param>
    /// <param name="errors">The errors encountered during the operation, if any.</param>
    /// <remarks>
    /// This constructor implicitly sets the operation's success status to <c>true</c>, indicating a sucessful operation.
    /// </remarks>
    public OperationResult(T? data, params Error[] errors) : this(true, data, errors)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OperationResult class, potentially with a set of errors. <br/>
    /// This constructor can represent either a successful or a failed operation based on the provided errors.
    /// </summary>
    /// <param name="errors">An array of <see cref="Error"/> objects representing the errors encountered during the operation. 
    /// If no errors are provided, the operation is considered successful by default.</param>
    /// <remarks>
    /// If errors are provided, the operation's success status is set to <c>false</c>, indicating a failed operation. <br/>
    /// If no errors are passed (an empty array), the constructor treats the operation as successful, setting the success status to <c>true</c>.
    /// </remarks>
    public OperationResult(params Error[] errors) : this(false, default, errors)
    {
        if (errors.Length == 0)
        {
            IsSuccess = true;
        }
    }

    /// <summary>
    /// Retrieves the data produced by the operation if it was successful. <br/>
    /// This method is a shorthand for avoiding null checks on the Data property and focuses on success and failure conditions.
    /// <br/><br/>
    /// <strong>Note:</strong> Always ensure that the operation was successful by checking <c>IsSuccess</c> before calling this method. <br/>
    /// Attempting to call <c>GetData</c> on a failed operation result will throw an exception.
    /// </summary>
    /// <returns>The data produced by the successful operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the operation result is not successful or when data is null.</exception>
    public T GetData()
    {
        if (Data == null)
        {
            throw new InvalidOperationException("Attempted to retrieve data from an unsuccessful operation result or when data is null.");
        }

        return Data;
    }

    /// <summary>
    /// Sets the data for the operation result and returns the current instance. <br/>
    /// This method is used to assign or update the operation's result data.
    /// </summary>
    /// <param name="data">The data to be set as the operation result.</param>
    /// <returns>The current <see cref="OperationResult{T}"/> instance with the updated data.</returns>
    public OperationResult<T> SetData(T? data)
    {
        Data = data;
        return this;
    }

}
