namespace ModularSystem.Core;

public class DefaultEntityConfiguration<T> : EntityConfiguration<T> where T : class, IQueryableModel
{
    public override ISerializer<T>? GetSerializer()
    {
        return new DefaultEntityJsonSerializer<T>();
    }
}