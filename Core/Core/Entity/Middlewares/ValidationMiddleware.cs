namespace ModularSystem.Core;

internal class ValidationMiddleware<TModel> : EntityMiddleware<TModel> where TModel : IQueryableModel
{
    private Entity<TModel> Entity { get; }

    public ValidationMiddleware(Entity<TModel> entity)
    {
        Entity = entity;
    }

    public override async Task BeforeCreateAsync(TModel entity)
    {
        if (Entity.Validator == null)
        {
            return;
        }

        await Entity.Hooks.BeforeValidateAsync(entity);
        await Entity.Validator.ValidateAsync(entity);
    }

    public override async Task BeforeUpdateAsync(TModel old, TModel @new)
    {
        if (Entity.UpdateValidator == null)
        {
            return;
        }

        await Entity.Hooks.BeforeValidateAsync(@new);
        await Entity.UpdateValidator.ValidateAsync(@new);
    }

    public override Task BeforeQueryAsync(IQuery<TModel> query)
    {
        if (Entity.QueryValidator == null)
        {
            return Task.CompletedTask;
        }

        return Entity.QueryValidator.ValidateAsync(query);
    }
}
