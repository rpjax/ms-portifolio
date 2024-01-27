using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Linq;

/// <summary>
/// A query provider that translates and executes LINQ expressions remotely for a specific data type.
/// </summary>
/// <typeparam name="T">The type of elements processed by the query.</typeparam>
public class ServiceQueryProvider<T> : IQueryProvider
{
    /// <summary>
    /// The client used to interact with the remote data service.
    /// </summary>
    protected internal QueryableClient Client { get; }

    /// <summary>
    /// Initializes a new instance of the ServiceQueryProvider class.
    /// </summary>
    /// <param name="client">The client to interact with the data source.</param>
    public ServiceQueryProvider(QueryableClient client)
    {
        Client = client;
    }

    /// <summary>
    /// Throws a NotSupportedException as non-generic query creation is not supported.
    /// </summary>
    /// <param name="expression">The expression for creating the query.</param>
    /// <returns>Does not return as it always throws NotSupportedException.</returns>
    /// <exception cref="NotSupportedException">Thrown to indicate that the non-generic query creation is not supported.</exception>
    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ServiceQueryable{TElement}"/> for the specified type.
    /// </summary>
    /// <typeparam name="TElement">The type of the data in the query.</typeparam>
    /// <param name="expression">The expression tree representing the query.</param>
    /// <returns>A new instance of <see cref="ServiceQueryable{TElement}"/> configured with the provided expression.</returns>
    /// <remarks>
    /// This method allows for creating type-safe and remotely executable queries.
    /// </remarks>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new ServiceQueryable<TElement>(expression, new ServiceQueryProvider<TElement>(Client));
    }

    /// <summary>
    /// Creates a new instance of <see cref="ServiceQueryable{T}"/> using a parameter expression.
    /// </summary>
    /// <returns>A new instance of <see cref="ServiceQueryable{T}"/>.</returns>
    /// <remarks>
    /// This method is a shortcut for creating a queryable for the specific type T.
    /// </remarks>
    public ServiceQueryable<T> CreateQuery()
    {
        var parameter = Expression.Parameter(typeof(IQueryable<T>));
        return (ServiceQueryable<T>)CreateQuery<T>(parameter);
    }

    /// <summary>
    /// Executes the provided expression synchronously and returns the result.
    /// </summary>
    /// <param name="expression">The expression to be executed.</param>
    /// <returns>The result of executing the expression.</returns>
    /// <remarks>
    /// This method executes the query remotely and synchronously by calling <see cref="ExecuteAsync(Expression)"/> and awaiting its result.
    /// </remarks>
    public object? Execute(Expression expression)
    {
        return ExecuteAsync(expression).Result;
    }

    /// <summary>
    /// Throws a NotSupportedException as the generic execute method is not supported.
    /// </summary>
    /// <typeparam name="TResult">The type of the result expected from the query execution.</typeparam>
    /// <param name="expression">The expression to be executed.</param>
    /// <returns>Does not return as it always throws NotSupportedException.</returns>
    /// <exception cref="NotSupportedException">Thrown to indicate that the generic execute method is not supported.</exception>
    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Asynchronously executes the provided expression and returns the result as an IQueryable of type T.
    /// </summary>
    /// <param name="expression">The expression to be executed.</param>
    /// <returns>A task that represents the asynchronous operation, with the result being an IQueryable of type T.</returns>
    /// <remarks>
    /// This method serializes the expression, sends it to the remote query service, and asynchronously retrieves the results.
    /// </remarks>
    public async Task<IQueryable<T>> ExecuteAsync(Expression expression)
    {
        var query = new SerializableQueryable(QueryProtocol.ToSerializable(expression));
        var result = await Client.QueryAsync<T[]>(query);

        return result.AsQueryable();
    }

}

/// <summary>
/// Custom exception for handling errors during query execution.
/// </summary>
public class QueryExecutionException : Exception
{
    public Error[] Errors { get; }

    public QueryExecutionException(string message, IEnumerable<Error> errors) : base(message)
    {
        Errors = errors.ToArray();
    }
}
