using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Extensions;
using ModularSystem.Core.Linq;
using ModularSystem.Core.Linq.Expressions;
using ModularSystem.EntityFramework.Linq;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.EntityFramework;


/*
 * 
 */

internal class QueryableBuilder<T>
{
    IQueryable<T> Queryable { get; set; }
    QueryReader<T> QueryReader { get; init; }

    public QueryableBuilder(IQueryable<T> queryable, IQuery<T> query)
    {
        Queryable = queryable;
        QueryReader = new(query);
    }

    public IQueryable<T> Create()
    {
        return Queryable;
    }

    public QueryableBuilder<T> UseFilter()
    {
        var predicate = QueryReader.GetFilterExpression();

        if (predicate != null)
        {
            Queryable = Queryable.Where(predicate);
        }

        return this;
    }

    public QueryableBuilder<T> UseGrouping()
    {
        // todo...
        return this;
    }

    public QueryableBuilder<T> UseProjection()
    {
        // todo...

        //*
        // NOTE: This code is commented because when the last code review was made, projection was not developed because of a problem in the Expression parser.
        //*

        //if (query.Projection != null)
        //{
        //    //var type = typeof(T);

        //    //if (type.IsAssignableFrom(typeof(ProductJoin)))
        //    //{
        //    //    Console.WriteLine();
        //    //}

        //    queryable = queryable.Select(query.Projection);
        //}
        return this;
    }

    public QueryableBuilder<T> UseOrdering()
    {
        var complexOrdering = QueryReader.GetOrderingExpression();
        var reader = new ComplexOrderingReader<T>(complexOrdering);

        if (complexOrdering != null)
        {
            foreach (var orderingExpression in reader.GetOrderingExpressions())
            {
                MethodInfo? methodInfo;

                var methodName =
                    orderingExpression.Direction == OrderingDirection.Descending
                    ? "OrderByDescending"
                    : "OrderBy";

                methodInfo = typeof(IOrderedEnumerable<T>)
                        .GetMethod(methodName)?
                        .MakeGenericMethod(orderingExpression.FieldType);

                if (methodInfo == null)
                {
                    throw new InvalidOperationException();
                }

                var modifiedQueryable = methodInfo
                    .Invoke(Queryable, new object[] { orderingExpression.FieldSelector })
                    ?.TryTypeCast<IQueryable<T>>();

                if (modifiedQueryable == null)
                {
                    throw new InvalidOperationException();
                }

                Queryable = modifiedQueryable;
            }
        }
        else
        {
            Queryable = Queryable.OrderBy(x => x);
        }

        return this;
    }

    public QueryableBuilder<T> UsePagination()
    {
        Queryable = Queryable
            .Skip(QueryReader.GetIntOffset())
            .Take(QueryReader.GetIntLimit());
        return this;
    }
}

internal class EFCoreUpdateOperation<T> where T : class, IEFCoreModel
{
    private DbSet<T> DbSet { get; }
    private IUpdateExpression UpdateExpression { get; }
    private bool AllowUpdatesWithNoFilter { get; }

    public EFCoreUpdateOperation(DbSet<T> dbSet, IUpdateExpression expression, bool allowUpdatesWithNoFilter)
    {
        DbSet = dbSet;
        AllowUpdatesWithNoFilter = allowUpdatesWithNoFilter;
    }

    public async Task<long> ExecuteAsync()
    {
        var reader = new UpdateExpressionReader<T>(UpdateExpression);
        var modifications = reader.GetModificationExpressions();

        if (modifications == null || modifications.IsEmpty())
        {
            return 0;
        }

        var queryable = DbSet.AsQueryable();
        var filterExpr = reader.GetFilterExpression();

        if (filterExpr == null)
        {
            if (!AllowUpdatesWithNoFilter)
            {
                throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the entity access object.");
            }
        }
        else
        {
            queryable = queryable.Where(filterExpr);
        }

        //*
        // Developer Notes:
        // The update function takes an expression argument of type:
        // Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>>
        // 
        // Where the updates are defined as:
        // (set) => set.SetProperty<T>(Func<T, TField> selector, T value)
        //
        // The 'set' object is primarily a LINQ placeholder and shouldn't be used directly.
        // This class is utilized internally by the Entity Framework to interpret updates.
        // This allows for flexible manipulation of the expression.
        //*
        var parameterExpression = Expression.Parameter(typeof(SetPropertyCalls<T>), "s");
        var fluentParameter = parameterExpression as Expression;
        var setCalls = new List<MethodCallExpression>();

        foreach (var updateSet in modifications)
        {
            //*
            // The LINQ method used to generate the update is:
            // new SetPropertyCalls<T>().SetProperty();
            //
            // NOTE: The method is the second element in the sequence produced by:
            // typeof(SetPropertyCalls<T>).GetMethods()
            //
            // If Entity Framework's API change this could require adaptation.
            //*
            var setMethodInfo = typeof(SetPropertyCalls<T>)
                .GetMethods()
                .Where(x => x.Name == "SetProperty")
                .ElementAt(1)
                .MakeGenericMethod(updateSet.FieldType);

            if (setMethodInfo == null)
            {
                throw new InvalidOperationException();
            }

            var selectorFuncDelegate = typeof(Func<,>)
                .MakeGenericType(new Type[] { typeof(T), updateSet.FieldType });
            var selectorExprDelegate = typeof(Expression<>)
                .MakeGenericType(selectorFuncDelegate);

            var selectorExpr = updateSet.ToLambda(Expression.Parameter(typeof(T), "x"));

            if (selectorExpr == null)
            {
                throw new Exception("Invalid or insupported UpdateSetExpression expression.");
            }

            var setMethodArgs = new Expression[]
            {
                selectorExpr,
                updateSet.Value
            };

            fluentParameter = Expression.Call(fluentParameter, setMethodInfo, setMethodArgs);
        }

        var lambdaExpression = Expression
            .Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(fluentParameter, parameterExpression);

        var visitor = new ParameterExpressionReferenceBinder();

        lambdaExpression = visitor.VisitAndConvert(lambdaExpression, "VisitLambda");

        return await queryable.ExecuteUpdateAsync(lambdaExpression);
    }

}