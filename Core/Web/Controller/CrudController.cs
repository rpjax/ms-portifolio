﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Security;
using ModularSystem.Web.Expressions;
using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Web;

/// <summary>
/// Generates a basic crud based on the modular system RESTful CRUD API specification. <br></br>
/// It uses the <see cref="IEntityService{T}"/> interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CrudController<T> : WebController, IPingController, IDisposable where T : class, IQueryableModel
{
    /// <summary>
    /// Gets the associated service instance for CRUD operations.
    /// </summary>
    protected abstract EntityService<T> Service { get; }

    /// <summary>
    /// Disposes the associated entity.
    /// </summary>
    [NonAction]
    public virtual void Dispose()
    {
        Service.Dispose();
        GC.SuppressFinalize(this);
    }

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

            var id = await Service.CreateAsync(data);
            return Ok(Dto<string>.From(id));
        }
        catch (Exception e)
        {
            return HandleException(e);
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
            return HandleException(e);
        }
    }

    [HttpPost("query")]
    public virtual async Task<IActionResult> QueryAsync([FromBody] SerializableQuery serializableQuery)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = serializableQuery.ToQuery<T>();
            var result = await Service.QueryAsync(query);
            return Ok(result);
        }
        catch (Exception e)
        {
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
        }
    }

}

/// <summary>
/// Extends the basic CRUD controller functionality to handle queryable data. <br/>
/// This controller supports querying operations using a queryable builder, allowing for complex querying logic on data sets.
/// </summary>
/// <typeparam name="T">The type of the entity that the controller manages. This type must implement <see cref="IQueryableModel"/>.</typeparam>
public abstract class QueryableController<T> : WebController where T : class, IQueryableModel
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
            var source = await Service.CreateQueryAsync();
            var queryable = VisitQueryable(request.ToQueryable(source));
            var data = await queryable.ToArrayAsync();

            return Ok(data);
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Disposes the associated entity.
    /// </summary>
    [NonAction]
    public virtual void Dispose()
    {
        Service.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the associated service instance for CRUD operations.
    /// </summary>
    protected abstract EntityService<T> Service { get; }

    /// <summary>
    /// Provides an extension point to modify or inspect the TranslatedQueryable object before it is executed. <br/>
    /// Override this method in derived classes to customize the query execution process.
    /// </summary>
    /// <param name="queryable">The translated queryable object to visit.</param>
    /// <returns>The potentially modified TranslatedQueryable object.</returns>
    protected virtual WebqlQueryable VisitQueryable(WebqlQueryable queryable)
    {
        return queryable;
    }
}

/// <summary>
/// Provides CRUD operations for WebQL queries. <br/>
/// This controller extends the basic CRUD functionalities to support WebQL query processing, allowing clients to query data using WebQL syntax.
/// </summary>
/// <typeparam name="T">The type of the entity that the controller manages. This type must implement <see cref="IQueryableModel"/>.</typeparam>
public abstract class WebqlController<T> : WebController where T : class, IQueryableModel
{
    //[HttpPost("webql-query")]
    //public async Task<IActionResult> QueryAsync()
    //{
    //    try
    //    {     
    //        var json = (await ReadBodyAsStringAsync()) ?? Translator.EmptyQuery;
    //        var translator = GetTranslator();
    //        var queryable = await Service.CreateQueryAsync();
    //        var transformedQueryable = VisitTranslatedQueryable(translator.TranslateToQueryable(json, queryable));
    //        var data = await transformedQueryable.ToArrayAsync();

    //        return Ok(data);
    //    }
    //    catch (Exception e)
    //    {
    //        if (e is ParseException parseException)
    //        {
    //            return HandleException(new AppException(parseException.GetMessage(), ExceptionCode.InvalidInput));
    //        }

    //        return HandleException(e);
    //    }
    //}

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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
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
            return HandleException(e);
        }
    }

    private LayerAdapter<TEntity, TPresented> CreateAdapter()
    {
        return new LayerAdapter<TEntity, TPresented>(Adapter);
    }
}
