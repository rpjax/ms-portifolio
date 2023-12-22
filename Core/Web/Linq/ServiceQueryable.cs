using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Web.Linq;

/// <summary>
/// Represents a queryable data structure for remote querying. <br/>
/// This class acts as a proxy, enabling LINQ queries to be executed not locally in-memory, <br/>
/// but remotely against a data service.
/// </summary>
public class ServiceQueryable : IQueryable
{
    /// <summary>
    /// Gets the element type of the data returned when executing the associated expression tree.
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// Gets the expression tree associated with this instance of IQueryable.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// Gets the query provider responsible for executing the query remotely.
    /// </summary>
    public IQueryProvider Provider { get; }

    /// <summary>
    /// Constructs a ServiceQueryable instance with specified element type, expression, and query provider.
    /// </summary>
    /// <remarks>
    /// The constructor sets up the necessary context for executing the query remotely,
    /// encapsulating the query logic and metadata.
    /// </remarks>
    public ServiceQueryable(Type elementType, Expression expression, IQueryProvider queryProvider)
    {
        ElementType = elementType;
        Expression = expression;
        Provider = queryProvider;
    }

    /// <summary>
    /// Executes the query remotely and returns an enumerator for iterating over the results.
    /// </summary>
    /// <remarks>
    /// The method delegates the query execution to the remote provider, ensuring 
    /// that the execution occurs against the actual data storage.
    /// </remarks>
    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)Provider.Execute(Expression)!).GetEnumerator();
    }
}

/// <summary>
/// A generic subclass of <see cref="ServiceQueryable"/> that facilitates type-safe querying capabilities for remote data sources.
/// </summary>
/// <remarks>
/// This class extends the functionality of <see cref="ServiceQueryable"/> by providing a type-safe way to <br/>
/// construct and execute queries against a remote service. It encapsulates the logic for <br/>
/// serializing LINQ expressions and sending them to a remote data service, <br/>
/// where the actual query execution takes place. This approach allows for the creation of <br/>
/// complex queries in a fluent and type-safe manner, ensuring that the execution and <br/>
/// data retrieval are handled efficiently by the remote data store.
/// </remarks>
/// <typeparam name="T">The type of the data being queried. This generic parameter ensures that <br/>
/// the queries constructed using this class are strongly typed, providing <br/>
/// compile-time type checking and IntelliSense support in IDEs.
/// </typeparam>
public class ServiceQueryable<T> : ServiceQueryable, IQueryable<T>
{
    /// <summary>
    /// Initializes a new instance of ServiceQueryable for the specified type.
    /// </summary>
    /// <param name="expression">The expression tree representing the query.</param>
    /// <param name="queryProvider">The provider that will execute the query remotely.</param>
    /// <remarks>
    /// This constructor allows for fluent, type-safe querying, maintaining compatibility
    /// with LINQ's standard query operators and custom extension methods like ToArrayAsync.
    /// </remarks>
    public ServiceQueryable(Expression expression, ServiceQueryProvider<T> queryProvider)
        : base(typeof(T), expression, queryProvider)
    {
    }

    /// <summary>
    /// Returns a type-safe enumerator for iterating over the query results.
    /// </summary>
    /// <remarks>
    /// This method enhances the type safety of query operations, ensuring that the results
    /// are consistent with the expected data type.
    /// </remarks>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)Provider.Execute(Expression)!).GetEnumerator();
    }
}
