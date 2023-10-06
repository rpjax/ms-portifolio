using System.Linq.Expressions;

namespace ModularSystem.Core;

// partial dedicated to commom components.
public partial class QueryReader<T>
{
    private Query<T> Query { get; }

    public QueryReader(Query<T> query)
    {
        Query = query;
    }

    public QueryReader(IQuery<T> query)
    {
        Query = new(query);
    }
}

// partial dedicated to filter reading.
public partial class QueryReader<T> 
{
    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        return Query.Filter as Expression<Func<T, bool>>;
    }
}

// partial dedicated to grouping reading.
public partial class QueryReader<T>
{
    // TODO...
}

// partial dedicated to projection reading.
public partial class QueryReader<T>
{
    // TODO...
}

// partial dedicated to ordering reading.
public partial class QueryReader<T>
{
    public OrderingExpression<T>? GetOrderingExpression()
    {
        return Query.Ordering as OrderingExpression<T>;
    }

    public Expression<Func<T, TField>>? GetOrderingSelectorExpression<TField>()
    {
        return GetOrderingExpression()?.FieldSelector as Expression<Func<T, TField>>;
    }

    public OrderingDirection GetOrderingDirection()
    {
        return Query.OrderingDirection;
    }
}

// partial dedicated to pagination reading.
public partial class QueryReader<T>
{
    public long Limit => GetLimit();
    public long Offset => GetOffset();
    public int IntLimit => GetIntLimit();
    public int IntOffset => GetIntOffset();
    public PaginationIn Pagination => GetPagination();

    public long GetLimit()
    {
        return Query.Pagination.Limit;
    }

    public long GetOffset()
    {
        return Query.Pagination.Offset;
    }

    public int GetIntLimit()
    {
        return Query.Pagination.IntLimit;
    }

    public int GetIntOffset()
    {
        return Query.Pagination.IntOffset;
    }

    public PaginationIn GetPagination()
    {
        return Query.Pagination;
    }
}