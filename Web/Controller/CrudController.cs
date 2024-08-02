using Microsoft.AspNetCore.Mvc;
using Aidan.Core;
using Aidan.Core.Linq.Extensions;

namespace Aidan.Web.Controllers;


/// <summary>
/// Base controller for performing CRUD operations on entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public abstract class CrudController<T> : WebController
{
    private IRepository<T> Repository { get; }

    public CrudController(IRepositoryProvider repositoryProvider)
    {
        Repository = repositoryProvider.GetRepository<T>();
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await Repository.CreateAsync(entity);

        return Ok();
    }

    /// <summary>
    /// Queries entities with optional limit and offset.
    /// </summary>
    /// <param name="limit">The maximum number of entities to retrieve.</param>
    /// <param name="offset">The number of entities to skip.</param>
    /// <returns>The queried entities.</returns>
    [HttpGet("query")]
    public virtual async Task<IActionResult> QueryAsync([FromQuery] int limit = 30, [FromQuery] int offset = 0)
    {
        var data = await Repository.AsQueryable()
            .Skip(offset)
            .Take(limit)
            .ToArrayAsync();

        return Ok(data);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    [HttpPut]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await Repository.UpdateAsync(entity);

        return Ok();
    }

    /// <summary>
    /// Deletes an existing entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromBody] T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await Repository.DeleteAsync(entity);

        return Ok();
    }

}
