using System.Linq.Expressions;

namespace ModularSystem.Core;

public class Update<T> : IUpdate<T> where T : IQueryableModel
{
    public Expression<Func<T, bool>>? Filter { get; set; } = null;
    public List<Expression>? Modifications { get; set; } = null;

    public Expression<Func<T, bool>>? FilterAsPredicate()
    {
        return Filter?.TryTypeCast<Expression<Func<T, bool>>>();
    }

    public List<T>? ModificationsAs<T>() where T : Expression
    {
        return Modifications?.ConvertAll(x => x.TypeCast<T>());  
    }

    public List<UpdateSet<T>>? ModificationsAsUpdateSets()
    {
        throw new NotImplementedException();
        //return Modifications?.ConvertAll(x =>
        //{
        //    var cast = x
        //});
    }

}

public class UpdateSet<T> where T : IQueryableModel
{
    public string FieldName { get; set; }
    public Type Type { get; }
    public dynamic? Value { get; set; }
}