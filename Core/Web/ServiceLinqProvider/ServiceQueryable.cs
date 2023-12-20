using ModularSystem.Core;
using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Web.Linq;

public class ServiceQueryable : IQueryable
{
    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    public ServiceQueryable(Type elementType, Expression expression, IQueryProvider queryProvider)
    {
        ElementType = elementType;
        Expression = expression;
        Provider = queryProvider;
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)Provider.Execute(Expression)!).GetEnumerator();
    }
}

public class ServiceQueryable<T> : ServiceQueryable, IQueryable<T>
{
    public ServiceQueryable(Expression expression, IQueryProvider queryProvider)
        : base(typeof(T), expression, queryProvider)
    {
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)Provider.Execute(Expression)!).GetEnumerator();
    }
}

public class ServiceQueryProvider<T> : IQueryProvider
{
    private QueryableClient Client { get; }

    public ServiceQueryProvider(QueryableClient client)
    {
        Client = client;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new ServiceQueryable(expression.Type.TryGetEnumerableType() ?? expression.Type, expression, this);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new ServiceQueryable<TElement>(expression, this);
    }

    public ServiceQueryable<T> CreateQuery()
    {
        return (ServiceQueryable<T>)CreateQuery<T>(Expression.Parameter(typeof(IQueryable<T>)));
    }

    public object? Execute(Expression expression)
    {
        var query = new SerializableQueryable(QueryProtocol.ToSerializable(expression));
        var response = Client.QueryAsync(query).Result;
        return response;
    }

    public TResult Execute<TResult>(Expression expression)
    {
        var query = new SerializableQueryable(QueryProtocol.ToSerializable(expression));
        var response = Client.QueryAsync(query).Result;
        return (TResult)response;
    }
}

public static class ServiceQueryableLinq
{
    public static ServiceQueryable<T> Where<T>(this ServiceQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
        return (ServiceQueryable<T>)queryable.AsQueryable().Where(predicate);
    }
}