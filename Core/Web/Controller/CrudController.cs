using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Security;
using ModularSystem.Mongo;
using ModularSystem.Web.Expressions;
using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;
using System.Text;
using System.Xml.Linq;

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

public class MyData : MongoModel
{
    public string FirstName { get; set; } = "";
    public string[] Surnames { get; set; } = new string[0];
    public string Cpf { get; set; } = "";
    public int Score { get; set; }
}

public abstract class WebQlController<T> : WebController where T : IQueryableModel
{
    protected abstract EntityService<T> Service { get; }
    
    private MyData[] Data = new MyData[]
        {
            new(){ Cpf = "11709620927", FirstName = "Amanda", Surnames = new[]{ "de", "Lima", "Santos" }, Score = 98 },
            new(){ Cpf = "11548128988", FirstName = "Rodrigo", Surnames = new[]{ "Pazzini", "Jacques" }, Score = 85 },
        };

    [HttpPost("query")]
    public async Task<IActionResult> QueryAsync()
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var json = await reader.ReadToEndAsync();
            var translator = new Translator();
            var queryable = translator.TranslateToQueryable(json, Data);
            var data = queryable.ToArray();
            var count = queryable.Count();

            var result = new QueryResult<object>()
            {
                Data = data,
                Pagination = new()
            };

            return Ok(result);
        }
        catch (Exception e)
        {
            if(e is ParseException parseException)
            {
                return HandleException(new AppException(parseException.GetMessage(), ExceptionCode.InvalidInput));
            }
            
            return HandleException(e);
        }
    }
}
