using ModularSystem.Core;
using MongoDB.Driver;

namespace ModularSystem.Mongo;

public delegate Task<IQueryResult<T>> SearchDelegate<T>(
    FilterDefinition<T> filter,
    PaginationIn pagination,
    SortDefinition<T>? sort,
    ProjectionDefinition<T>? projection
) where T : IMongoModel;

public class MongoSearch<T> where T : IMongoModel
{
    private SearchDelegate<T> SearchAsync { get; }
    private QueryReader<T> Reader { get; }

    public MongoSearch(SearchDelegate<T> searchDelegate, IQuery<T> query)
    {
        SearchAsync = searchDelegate;
        Reader = new(query);
    }

    public Task<IQueryResult<T>> RunAsync()
    {
        FilterDefinition<T>? filter = null;
        SortDefinition<T>? sort = null;

        var predicate = Reader.GetFilterExpression();

        if (predicate != null)
        {
            filter = MongoModule.GetFilterBuilder<T>().Where(predicate);
        }
        else
        {
            filter = MongoModule.GetFilterBuilder<T>().Empty;
        }

        var ordering = Reader.GetOrderingExpression();

        if (ordering != null)
        {
            if (Reader.GetOrderingDirection() == OrderingDirection.Ascending)
            {
                sort = Builders<T>.Sort.Ascending(ordering.FieldName);
            }
            else
            {
                sort = Builders<T>.Sort.Descending(ordering.FieldName);
            }
        }
        else
        {
            sort = Builders<T>.Sort.Ascending(x => x.CreatedAt);
        }

        return SearchAsync(filter, Reader.Pagination, sort, null);
    }

}