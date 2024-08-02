using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Aidan.Core;
using Aidan.Core.Extensions;
using Aidan.Core.Linq.Expressions;
using System.Linq.Expressions;

namespace Aidan.EntityFramework;

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
        throw new NotImplementedException();
        //var reader = new UpdateExpressionReader<T>(UpdateExpression);
        //var modifications = reader.GetModificationExpressions();

        //if (modifications == null || modifications.IsEmpty())
        //{
        //    return 0;
        //}

        //var queryable = DbSet.AsQueryable();
        //var filterExpr = reader.GetFilterExpression();

        //if (filterExpr == null)
        //{
        //    if (!AllowUpdatesWithNoFilter)
        //    {
        //        throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the entity access object.");
        //    }
        //}
        //else
        //{
        //    queryable = queryable.Where(filterExpr);
        //}

        ////*
        //// Developer Notes:
        //// The update function takes an expression argument of type:
        //// Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>>
        //// 
        //// Where the updates are defined as:
        //// (set) => set.SetProperty<T>(Func<T, TField> selector, T value)
        ////
        //// The 'set' object is primarily a LINQ placeholder and shouldn't be used directly.
        //// This class is utilized internally by the Entity Framework to interpret updates.
        //// This allows for flexible manipulation of the expression.
        ////*
        //var parameterExpression = Expression.Parameter(typeof(SetPropertyCalls<T>), "s");
        //var fluentParameter = parameterExpression as Expression;
        //var setCalls = new List<MethodCallExpression>();

        //foreach (var updateSet in modifications)
        //{
        //    //*
        //    // The LINQ method used to generate the update is:
        //    // new SetPropertyCalls<T>().SetProperty();
        //    //
        //    // NOTE: The method is the second element in the sequence produced by:
        //    // typeof(SetPropertyCalls<T>).GetMethods()
        //    //
        //    // If Entity Framework's API change this could require adaptation.
        //    //*
        //    var setMethodInfo = typeof(SetPropertyCalls<T>)
        //        .GetMethods()
        //        .Where(x => x.Name == "SetProperty")
        //        .ElementAt(1)
        //        .MakeGenericMethod(updateSet.FieldType);

        //    if (setMethodInfo == null)
        //    {
        //        throw new InvalidOperationException();
        //    }

        //    var selectorFuncDelegate = typeof(Func<,>)
        //        .MakeGenericType(new Type[] { typeof(T), updateSet.FieldType });
        //    var selectorExprDelegate = typeof(Expression<>)
        //        .MakeGenericType(selectorFuncDelegate);

        //    var selectorExpr = updateSet.ToLambda(Expression.Parameter(typeof(T), "x"));

        //    if (selectorExpr == null)
        //    {
        //        throw new Exception("Invalid or insupported UpdateSetExpression expression.");
        //    }

        //    var setMethodArgs = new Expression[]
        //    {
        //        selectorExpr,
        //        updateSet.Value
        //    };

        //    fluentParameter = Expression.Call(fluentParameter, setMethodInfo, setMethodArgs);
        //}

        //var lambdaExpression = Expression
        //    .Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(fluentParameter, parameterExpression);

        //var visitor = new ParameterExpressionReferenceBinder();

        //lambdaExpression = visitor.VisitAndConvert(lambdaExpression, "VisitLambda");

        //return await queryable.ExecuteUpdateAsync(lambdaExpression);
    }

}