using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Linq;
using ModularSystem.Core.Security;
using ModularSystem.Web.Expressions;
using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Web;

/// <summary>
/// Provides a base controller for services handling data of type <typeparamref name="T"/>. <br/>
/// This abstract controller defines common functionalities for service-oriented controllers.
/// </summary>
/// <typeparam name="T">The type of the data model, which must implement <see cref="IQueryableModel"/>.</typeparam>
public abstract class ServiceController<T> : WebController where T : IQueryableModel
{
    /// <summary>
    /// Represents the abstract property for accessing the EntityService associated with the specific data type T. <br/>
    /// This service provides the necessary operations for handling data entities in the underlying data store.
    /// </summary>
    protected abstract EntityService<T> Service { get; }

    /// <summary>
    /// Disposes of the resources used by the ServiceController. <br/>
    /// This method ensures that the EntityService is properly disposed, and GC.SuppressFinalize is called to optimize garbage collection.
    /// </summary>
    [NonAction]
    public virtual void Dispose()
    {
        Service.Dispose();
        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Defines a controller to implement basic CRUD (Create, Read, Update, Delete) operations based on a RESTful API design.<br/>
/// This controller interacts with data of type <typeparamref name="T"/> using <see cref="EntityService{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the data entity. Must be a class implementing <see cref="IQueryableModel"/>.</typeparam>
public abstract class CrudController<T> : ServiceController<T>, IPingController, IDisposable where T : class, IQueryableModel
{
    // CREATE
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] T data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dto = Dto<string>
                .From(await Service.CreateAsync(data));

            return Ok(dto);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    // READ
    [HttpGet]
    public virtual async Task<IActionResult> GetByIdAsync(string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await Service.GetAsync(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpPost("query")]
    public virtual async Task<IActionResult> QueryAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = (await DeserializeBodyAsJsonAsync<SerializableQuery>()) ?? new();
            var query = request.ToQuery<T>();
            var data = await Service.QueryAsync(query);

            return Ok(data);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }

    }

    [HttpPost("queryable-query")]
    public async Task<IActionResult> QueryAsync([FromBody] SerializableQueryable request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var source = await Service.CreateQueryAsync();
            var queryable = VisitQueryable(request.ToQueryable(source));
            var data = queryable.ToArray();

            return Ok(data);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    // UPDATE
    [HttpPut]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] T data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Service.UpdateAsync(data);

            return Ok();
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpPatch("bulk-update")]
    public virtual async Task<IActionResult> BulkUpdateAsync([FromBody] SerializableUpdate serializableUpdate)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var affectedRecords = await Service.UpdateAsync(serializableUpdate.ToUpdate<T>());
            var dto = new Dto<long?>(affectedRecords);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    // DELETE
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync(string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Service.DeleteAsync(id);

            return Ok();
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpPatch("bulk-delete")]
    public virtual async Task<IActionResult> BulkDeleteAsync([FromBody] SerializableExpression serializableExpression)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reader = new ExpressionReader(QueryProtocol.FromSerializable(serializableExpression));
            var affectedRecords = await Service.DeleteAsync(reader.GetPredicate<T>());
            var dto = new Dto<long?>(affectedRecords);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    //*
    // End of CRUD section.
    //*

    [HttpGet("ping")]
    public virtual async Task<IActionResult> Ping()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok("pong!");
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpGet("id-validation")]
    public virtual async Task<IActionResult> ValidateIdAsync(string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isValid = await Service.ValidateIdAsync(id);
            return Ok(Dto<bool>.From(isValid));
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpGet("count")]
    public virtual async Task<IActionResult> CountAsync([FromBody] SerializableExpression serializableExpression)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reader = new ExpressionReader(QueryProtocol.FromSerializable(serializableExpression));
            var count = await Service.CountAsync(reader.GetPredicate<T>());
            var dto = new Dto<long?>(count);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }


    /// <summary>
    /// Provides an extension point to modify or inspect the TranslatedQueryable object before it is executed. <br/>
    /// Override this method in derived classes to customize the query execution process.
    /// </summary>
    /// <param name="queryable">The translated queryable object to visit.</param>
    /// <returns>The potentially modified TranslatedQueryable object.</returns>
    protected virtual IQueryable<object> VisitQueryable(IQueryable<object> queryable)
    {
        return queryable;
    }

}

/// <summary>
/// Enhances the base service controller by enabling complex querying functionalities. <br/>
/// It processes dynamic queries defined by <see cref="SerializableQueryable"/>.
/// </summary>
/// <typeparam name="T">The type of the entity being managed, conforming to <see cref="IQueryableModel"/>.</typeparam>
/// <returns>A task resulting in an IActionResult containing the query results or an error message.</returns>
public abstract class QueryableController<T> : ServiceController<T> where T : class, IQueryableModel
{
    /// <summary>
    /// Handles an incoming query request and returns the result of the query. <br/>
    /// This method processes a query defined by a <see cref="SerializableQueryable"/>, allowing for dynamic querying capabilities.
    /// </summary>
    /// <param name="request">The serialized queryable that defines the query logic.</param>
    /// <returns>An IActionResult containing the query results or an error message.</returns>
    [HttpPost("queryable-query")]
    public async Task<IActionResult> QueryAsync([FromBody] SerializableQueryable request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
   
            var source = await Service.CreateQueryAsync();
            var queryable = VisitQueryable(request.ToQueryable(source));
            var data = queryable.ToArray();

            return Ok(data);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    /// <summary>
    /// Provides an extension point to modify or inspect the TranslatedQueryable object before it is executed. <br/>
    /// Override this method in derived classes to customize the query execution process.
    /// </summary>
    /// <param name="queryable">The translated queryable object to visit.</param>
    /// <returns>The potentially modified TranslatedQueryable object.</returns>
    protected virtual IQueryable<object> VisitQueryable(IQueryable<object> queryable)
    {
        return queryable;
    }

}

/// <summary>
/// Extends service controller functionalities to handle WebQL queries. <br/>
/// Enables data querying using WebQL syntax for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type managed by the controller, implementing <see cref="IQueryableModel"/>.</typeparam>
/// <returns>A task resulting in an IActionResult with the processed data or an error message.</returns>
public abstract class WebqlController<T> : WebController
{
    [HttpPost("query")]
    public virtual async Task<IActionResult> QueryAsync()
    {
        try
        {
            var json = (await ReadBodyAsStringAsync()) ?? Translator.EmptyQuery;
            var translator = GetTranslator();
            var source = VisitSource(await CreateQueryAsync());
            var webqlQueryable = VisitQueryable(translator.TranslateToQueryable(json, source));
            var data = await webqlQueryable.ToArrayAsync();

            return Ok(data);
        }
        catch (Exception e)
        {
            if (e is ParseException parseException)
            {
                return ExceptionResponse(new AppException(parseException.GetMessage(), ExceptionCode.InvalidInput));
            }

            return ExceptionResponse(e);
        }
    }

    protected abstract Task<IQueryable<T>> CreateQueryAsync();

    /// <summary>
    /// Gets the WebQL translator to translate queries.
    /// </summary>
    /// <returns></returns>
    protected virtual Translator GetTranslator()
    {
        return new Translator(GetTranslatorOptions());
    }

    /// <summary>
    /// Sets the options used by the translator to interpret WebQL queries.
    /// </summary>
    /// <returns></returns>
    protected virtual TranslatorOptions GetTranslatorOptions()
    {
        return new TranslatorOptions();
    }

    /// <summary>
    /// Provides a method to modify or inspect a ServiceQueryable object before its execution. <br/>
    /// This method serves as an extension point in derived classes, allowing for customization of the query execution process.
    /// </summary>
    /// <param name="source">The ServiceQueryable object representing the query to be executed.</param>
    /// <returns>A potentially modified ServiceQueryable object that is ready for execution.</returns>
    protected virtual IQueryable<T> VisitSource(IQueryable<T> source)
    {
        return source;
    }

    /// <summary>
    /// Provides a method to modify or inspect a WebqlQueryable object before its execution. <br/>
    /// This virtual method serves as an extension point in derived classes, allowing for customization of the query execution process.
    /// </summary>
    /// <param name="queryable">The WebqlQueryable object representing the query to be executed.</param>
    /// <returns>A potentially modified WebqlQueryable object that is ready for execution.</returns>
    protected virtual WebqlQueryable VisitQueryable(WebqlQueryable queryable)
    {
        return queryable;
    }

}

/// <summary>
/// Generates a basic crud based on the modular system RESTful CRUD API specification with an adapter layer.<br></br>
/// It uses the <see cref="IEntityService{T}"/> interface.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TPresented"></typeparam>
[Obsolete("As of version 1.40.0, the CrudController with an adapter layer is deprecated due to its tight coupling with the type-based modeling of expression tree operations across applications. This behavior is primarily introduced by the QueryProtocol class. Consider using alternative approaches.")]
public abstract class CrudController<TEntity, TPresented> : WebController, IPingController, IDisposable where TEntity : class, IQueryableModel where TPresented : class
{
    /// <summary>
    /// Gets the associated entity instance for CRUD operations.
    /// </summary>
    protected abstract EntityService<TEntity> Entity { get; }

    /// <summary>
    /// Gets the associated adapter for converting between entity and presentation layers.
    /// </summary>
    protected abstract ILayerAdapter<TEntity, TPresented> Adapter { get; }

    /// <summary>
    /// Disposes the associated entity.
    /// </summary>
    public void Dispose()
    {
        Entity.Dispose();
    }

    // CREATE
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] TPresented data, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var instance = Adapter.Adapt(data);
            var id = await Entity.CreateAsync(instance);

            return Ok(Dto<string>.From(id));
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    // READ
    [HttpGet]
    public virtual async Task<IActionResult> GetByIdAsync(string id, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var entity = Entity;
            var data = await entity.GetAsync(id);
            var presentedData = Adapter.Present(data);

            return Ok(presentedData);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpPost("query")]
    public virtual async Task<IActionResult> QueryAsync([FromBody] SerializableQuery serializedQuery, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var query = serializedQuery.ToQuery<TEntity>();
            var result = await Entity.QueryAsync(query);
            var presented = CreateAdapter().Present(result);
            return Ok(presented);
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }

    }

    // UPDATE
    [HttpPut]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] TPresented data, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var input = Adapter.Adapt(data);
            await Entity.UpdateAsync(input);
            return Ok();
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    // DELETE
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync(string id, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            await Entity.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    //*
    // End of CRUD section.
    //*

    [HttpGet("ping")]
    public virtual async Task<IActionResult> Ping()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok("pong!");
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    [HttpGet("id-validation")]
    public virtual async Task<IActionResult> ValidateIdAsync(string id, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var isValid = await Entity.ValidateIdAsync(id);
            return Ok(Dto<bool>.From(isValid));
        }
        catch (Exception e)
        {
            return ExceptionResponse(e);
        }
    }

    private LayerAdapter<TEntity, TPresented> CreateAdapter()
    {
        return new LayerAdapter<TEntity, TPresented>(Adapter);
    }
}
