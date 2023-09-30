namespace ModularSystem.Core;

internal class ValidationMiddleware<T> : EntityMiddleware<T> where T : IQueryableModel
{
    private Entity<T> Entity { get; }

    public ValidationMiddleware(Entity<T> entity)
    {
        Entity = entity;
    }

    public override Task<T> BeforeCreateAsync(T entity)
    {
        return base.BeforeCreateAsync(entity);
    }

    public override Task<(T, T)> BeforeUpdateAsync(T old, T @new)
    {
        return base.BeforeUpdateAsync(old, @new);
    }

    //public override async Task BeforeCreateAsync(TModel entity)
    //{
    //    if (Entity.Validator == null)
    //    {
    //        return;
    //    }

    //    await Entity.Hooks.BeforeValidateAsync(entity);
    //    await Entity.Validator.ValidateAsync(entity);
    //}

    //public override async Task BeforeUpdateAsync(TModel old, TModel @new)
    //{
    //    if (Entity.UpdateValidator == null)
    //    {
    //        return;
    //    }

    //    await Entity.Hooks.BeforeValidateAsync(@new);
    //    await Entity.UpdateValidator.ValidateAsync(@new);
    //}

    //public override Task BeforeQueryAsync(IQuery<TModel> query)
    //{
    //    if (Entity.QueryValidator == null)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    return Entity.QueryValidator.ValidateAsync(query);
    //}
}
