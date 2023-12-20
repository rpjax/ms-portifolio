using ModularSystem.Core;
using ModularSystem.Web.Client;
using System.Linq.Expressions;

namespace ModularSystem.Web;

/// <summary>
/// Represents a web client that handles API operations through defined endpoints.
/// </summary>
public class WebClient
{
    /// <summary>
    /// Gets the endpoint configuration used for API requests.
    /// </summary>
    protected EndpointConfiguration Config { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebClient"/> class with the specified endpoint configuration.
    /// </summary>
    /// <param name="config">The <see cref="EndpointConfiguration"/> used for initializing the web client.</param>
    public WebClient(EndpointConfiguration config)
    {
        Config = config;
    }

    /// <summary>
    /// Sends a ping request to the server asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method sends a ping request to the server to check its availability.
    /// It uses the <see cref="PingEndpoint"/> class for performing the operation.
    /// </remarks>
    public virtual Task PingAsync()
    {
        var endpoint = new PingEndpoint(Config.RequestUri.Copy());
        return endpoint.RunAsync();
    }

    /// <summary>
    /// Returns a deep copy of the original <see cref="EndpointConfiguration"/> object.
    /// </summary>
    /// <returns>A deep copy of the <see cref="EndpointConfiguration"/> object.</returns>
    /// <remarks>
    /// This ensures that the URI in the configuration can be modified freely without affecting the original configuration.
    /// </remarks>
    protected virtual EndpointConfiguration CopyConfig()
    {
        return Config.Copy();
    }

    /// <summary>
    /// Returns a copy of the original URI object.
    /// </summary>
    /// <returns>A copy of the <see cref="Http.Uri"/> object.</returns>
    /// <remarks>
    /// This allows the URI to be modified for individual requests without altering the original URI in the configuration.
    /// </remarks>
    protected virtual Http.Uri CopyUri()
    {
        return Config.RequestUri.Copy();
    }
}

/// <summary>
/// Provides CRUD (Create, Read, Update, Delete) operations for a given type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object that this client operates on. Must be a class.</typeparam>
public class CrudClient<T> : WebClient where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CrudClient{T}"/> class with the specified endpoint configuration.
    /// </summary>
    /// <param name="config">The <see cref="EndpointConfiguration"/> used for initializing the CRUD client.</param>
    public CrudClient(EndpointConfiguration config) : base(config)
    {

    }

    /// <summary>
    /// Asynchronously creates a new instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The instance to be created.</param>
    /// <returns>The ID of the created instance as a string.</returns>
    public async Task<string> CreateAsync(T value)
    {
        var endpoint = new CreateEndpoint<T>(CopyUri());
        var dto = await endpoint.RunAsync(value);
        return dto.Value ?? string.Empty;
    }

    /// <summary>
    /// Asynchronously retrieves an instance of type <typeparamref name="T"/> by its ID.
    /// </summary>
    /// <param name="id">The ID of the instance.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the retrieved instance.</returns>
    public Task<T> GetByIdAsync(string id)
    {
        var endpoint = new GetByIdEndpoint<T>(CopyUri());
        return endpoint.RunAsync(id);
    }

    /// <summary>
    /// Asynchronously queries instances of type <typeparamref name="T"/> based on a search query.
    /// </summary>
    /// <param name="query">The serializable query for the search.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the query.</returns>
    public Task<QueryResult<T>> QueryAsync(SerializableQuery query)
    {
        return new QueryEndpoint<T>(CopyUri()).RunAsync(query);
    }

    /// <summary>
    /// Asynchronously queries entities of type <typeparamref name="T"/> based on a search query.
    /// </summary>
    /// <param name="query">The query defining the search criteria.</param>
    /// <returns>A task representing the asynchronous query operation, with a result containing the matched entities.</returns>
    public Task<QueryResult<T>> QueryAsync(Query<T> query)
    {
        return QueryAsync(query.ToSerializable());
    }

    /// <summary>
    /// Asynchronously updates an existing instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The instance to be updated.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public async Task UpdateAsync(T value)
    {
        var endpoint = new UpdateEndpoint<T>(CopyUri());
        await endpoint.RunAsync(value);
    }

    /// <summary>
    /// Asynchronously updates entities based on the provided update criteria.
    /// </summary>
    /// <param name="update">The serialized update criteria.</param>
    /// <returns>The number of entities updated; null if the update count is unavailable.</returns>
    public async Task<long?> UpdateAsync(SerializableUpdate update)
    {
        var dto = await new UpdateBulkEndpoint(CopyUri()).RunAsync(update);
        return dto.Value;
    }

    /// <summary>
    /// Asynchronously updates multiple entities of type <typeparamref name="T"/> based on the provided update criteria.
    /// </summary>
    /// <param name="update">The update criteria specifying which entities to update and the modifications to apply.</param>
    /// <returns>A task representing the asynchronous bulk update operation, with a result indicating the number of entities updated. Returns null if the update count is not available.</returns>
    public Task<long?> BulkUpdateAsync(Update<T> update)
    {
        return UpdateAsync(update.ToSerializable());
    }

    /// <summary>
    /// Asynchronously deletes an instance of type <typeparamref name="T"/> by its ID.
    /// </summary>
    /// <param name="id">The ID of the instance.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public Task DeleteByIdAsync(string id)
    {
        return new DeleteByIdEndpoint<T>(CopyUri()).RunAsync(id);
    }

    /// <summary>
    /// Deletes multiple entities of type <typeparamref name="T"/> based on the provided criteria.
    /// </summary>
    /// <param name="expression">The criteria to identify entities to delete.</param>
    /// <returns>The number of entities deleted; null if the delete count is unavailable.</returns>
    public async Task<long?> BulkDeleteAsync(Expression<Func<T, bool>> expression)
    {
        var serializable = QueryProtocol.ToSerializable(expression);
        var endpoint = new BulkDeleteEndpoint(CopyUri());
        var dto = await endpoint.RunAsync(serializable);
        return dto.Value;
    }

    /// <summary>
    /// Asynchronously validates the ID of an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>A task representing the asynchronous validation operation, with a result of true if the ID is valid; otherwise, false.</returns>
    public async Task<bool> ValidateIdAsync(string id)
    {
        var endpoint = new ValidateIdEndpoint<T>(CopyUri());
        var dto = await endpoint.RunAsync(id);
        return dto.Value;
    }
}

public class QueryableClient : WebClient
{
    public QueryableClient(EndpointConfiguration config) : base(config)
    {
    }

    /// <summary>
    /// Asynchronously queries instances of type <typeparamref name="T"/> based on a search query.
    /// </summary>
    /// <param name="query">The serializable query for the search.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the query.</returns>
    public async Task<IQueryable<object>> QueryAsync(SerializableQueryable query)
    {
        var data = await new QueryableEndpoint(CopyUri())
            .RunAsync(query);

        return (IQueryable<object>)data;
    }

}

public class QueryableReader
{

}