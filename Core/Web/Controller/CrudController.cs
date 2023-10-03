using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core;
using ModularSystem.Core.Security;

namespace ModularSystem.Web;

/// <summary>
/// Generates a basic crud based on the modular system RESTful CRUD API specification. <br></br>
/// It uses the <see cref="IEntityService{T}"/> interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CrudController<T> : WebController, IPingController, IDisposable where T : class, IQueryableModel
{
    /// <summary>
    /// Gets the associated entity instance for CRUD operations.
    /// </summary>
    protected abstract EntityService<T> Entity { get; }

    /// <summary>
    /// Disposes the associated entity.
    /// </summary>
    public virtual void Dispose()
    {
        Entity.Dispose();
        GC.SuppressFinalize(this);
    }

    // CREATE
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] T data, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var id = await Entity.CreateAsync(data);
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
            var data = await Entity.GetAsync(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpPost("query")]
    public virtual async Task<IActionResult> QueryAsync([FromBody] SerializedQuery serializedQuery, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            var query = serializedQuery.ToQuery<T>();
            var result = await Entity.QueryAsync(query);
            return Ok(result);
        }
        catch (Exception e)
        {
            return HandleException(e);
        }

    }

    // UPDATE
    [HttpPut]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] T data, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
            await Entity.UpdateAsync(data);
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
    public virtual async Task<IActionResult> Ping([BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
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

    protected virtual IQuery<T> CreateSearch(SerializedQuery input)
    {
        return input.ToQuery<T>();
    }
}

/// <summary>
/// Generates a basic crud based on the modular system RESTful CRUD API specification with an adapter layer.<br></br>
/// It uses the <see cref="IEntityService{T}"/> interface.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TPresented"></typeparam>
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
    public virtual async Task<IActionResult> QueryAsync([FromBody] SerializedQuery serializedQuery, [BindNever] IResourcePolicy? resourcePolicy = null)
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
    public virtual async Task<IActionResult> Ping([BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await AuthorizeAsync(resourcePolicy);
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