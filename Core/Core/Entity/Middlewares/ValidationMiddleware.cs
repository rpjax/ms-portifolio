using System.Text;

namespace ModularSystem.Core;

/// <summary>
/// Middleware responsible for enforcing validation logic on entities during various operations.
/// </summary>
/// <remarks>
/// The validation logic is derived from the following properties of the <see cref="EntityService{T}"/>:
/// <list type="bullet">
/// <item><description><see cref="EntityService{T}.Validator"/> for entity creation and bulk creation.</description></item>
/// <item><description><see cref="EntityService{T}.UpdateValidator"/> for entity updates.</description></item>
/// <item><description><see cref="EntityService{T}.QueryValidator"/> for querying entities.</description></item>
/// </list>
/// </remarks>
/// <typeparam name="T">The type of the entity being validated.</typeparam>
internal class ValidationMiddleware<T> : EntityMiddleware<T> where T : IQueryableModel
{
    private EntityService<T> Service { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationMiddleware{T}"/> class.
    /// </summary>
    /// <param name="entity">The entity service containing the validation logic.</param>
    public ValidationMiddleware(EntityService<T> entity)
    {
        Service = entity;
    }

    /// <summary>
    /// Validates the entity before creation.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validated entity.</returns>
    public override async Task<T> BeforeCreateAsync(T entity)
    {
        if (Service.Validator != null)
        {
            var error = await Service.Validator.ValidateAsync(entity);

            if (error != null)
            {
                throw error;
            }
        }

        return await base.BeforeCreateAsync(entity);
    }

    /// <summary>
    /// Validates a collection of entities before bulk creation.
    /// </summary>
    /// <param name="entities">The collection of entities to validate.</param>
    /// <returns>The validated entities.</returns>
    /// <remarks>
    /// If multiple validation errors are detected, an aggregated exception is thrown containing all individual validation errors.
    /// </remarks>
    public override async Task<IEnumerable<T>> BeforeCreateAsync(IEnumerable<T> entities)
    {
        if(Service.Validator == null)
        {
            return await base.BeforeCreateAsync(entities);
        }

        var validationTasks = new List<Task<Exception?>>();

        foreach (var entity in entities)
        {
            validationTasks.Add(Service.Validator.ValidateAsync(entity));
        }

        var validationResults = await Task.WhenAll(validationTasks);
        var errors = validationResults.Where(e => e != null).Select(e => e!).ToArray();

        if (errors.IsEmpty())
        {
            return await base.BeforeCreateAsync(entities);
        }
        if(errors.Length == 1)
        {
            throw errors.First();
        }

        var exception = new AppException("Bulk create operation threw multiple validation errors.", ExceptionCode.InvalidInput, null, errors);

        return await base.BeforeCreateAsync(entities);
    }

    /// <summary>
    /// Validates the entity before updating.
    /// </summary>
    /// <param name="currentValue">The current state of the entity.</param>
    /// <param name="updatedValue">The new state of the entity.</param>
    /// <returns>The validated current and updated entities.</returns>
    public override async Task<(T, T)> BeforeUpdateAsync(T currentValue, T updatedValue)
    {
        if (Service.UpdateValidator != null)
        {
            var error = await Service.UpdateValidator.ValidateAsync(updatedValue);

            if (error != null)
            {
                throw error;
            }
        }

        return await base.BeforeUpdateAsync(currentValue, updatedValue);
    }

    /// <summary>
    /// Validates the query before execution.
    /// </summary>
    /// <param name="query">The query to validate.</param>
    /// <returns>The validated query.</returns>
    public override async Task<IQuery<T>> BeforeQueryAsync(IQuery<T> query)
    {
        if (Service.QueryValidator != null)
        {
            var error = await Service.QueryValidator.ValidateAsync(query);

            if (error != null)
            {
                throw error;
            }
        }

        return await base.BeforeQueryAsync(query);
    }
}
