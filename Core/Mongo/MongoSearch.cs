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
    protected SearchDelegate<T> SearchAsync { get; }

    public MongoSearch(SearchDelegate<T> searchDelegate)
    {
        SearchAsync = searchDelegate;
    }

    public Task<IQueryResult<T>> RunAsync(IQuery<T> query)
    {
        FilterDefinition<T>? filter = null;
        SortDefinition<T>? sort = null;

        if (query.Filter != null)
        {
            filter = MongoModule.GetFilterBuilder<T>().Where(query.Filter);
        }
        else
        {
            filter = MongoModule.GetFilterBuilder<T>().Empty;
        }

        if (query.Sort != null)
        {
            if (query.Order == Ordering.Ascending)
            {
                sort = Builders<T>.Sort.Ascending(query.Sort);
            }
            else
            {
                sort = Builders<T>.Sort.Descending(query.Sort);
            }
        }
        else
        {
            sort = Builders<T>.Sort.Ascending(x => x.CreatedAt);
        }

        return SearchAsync(filter, query.Pagination, sort, null);
    }

}