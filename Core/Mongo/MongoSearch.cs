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
        SortDefinition<T>? sortDefinition = null;

        var predicate = Reader.GetFilterExpression();

        if (predicate != null)
        {
            filter = MongoModule.GetFilterBuilder<T>().Where(predicate);
        }
        else
        {
            filter = MongoModule.GetFilterBuilder<T>().Empty;
        }

        var complexOrdering = Reader.GetOrderingExpression();
        var orderingReader = new ComplexOrderingReader<T>(complexOrdering);

        if (complexOrdering != null)
        {
            foreach (var orderingExpression in orderingReader.GetOrderingExpressions())
            {
                var newOrder = null as SortDefinition<T>;

                if (orderingExpression.Direction == OrderingDirection.Ascending)
                {
                    newOrder = Builders<T>.Sort.Ascending(orderingExpression.FieldName);
                }
                else
                {
                    newOrder = Builders<T>.Sort.Descending(orderingExpression.FieldName);
                }

                sortDefinition = Builders<T>.Sort.Combine(sortDefinition, newOrder);
            }
        }
        else
        {
            sortDefinition = Builders<T>.Sort.Ascending(x => x.CreatedAt);
        }

        return SearchAsync(filter, Reader.Pagination, sortDefinition, null);
    }

}