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
internal class ValidationMiddleware<T> : EntityMiddleware<T> where T : IEntity
{
    private EntityService<T> Service { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationMiddleware{T}"/> class.
    /// </summary>
    /// <param name="service">The entity service containing the validation logic.</param>
    public ValidationMiddleware(EntityService<T> service)
    {
        Service = service;
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
            var result = Service.Validator.Validate(entity);

            if (result.IsFailure)
            {
                throw new ErrorException(result);
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
        if (Service.Validator == null)
        {
            return await base.BeforeCreateAsync(entities);
        }

        foreach (var entity in entities)
        {
            var result = Service.Validator.Validate(entity);

            if (result.IsFailure)
            {
                throw new ErrorException(result);
            }
        }

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
            var result = Service.UpdateValidator.Validate(updatedValue);

            if (result.IsFailure)
            {
                throw new ErrorException(result);
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
            var result = Service.QueryValidator.Validate(query);

            if (result.IsFailure)
            {
                throw new ErrorException(result);
            }
        }

        return await base.BeforeQueryAsync(query);
    }
}
