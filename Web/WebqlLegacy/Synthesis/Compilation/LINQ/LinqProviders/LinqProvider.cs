using Aidan.Core.Linq;
using Aidan.Webql.Analysis.Semantics;
using Aidan.Webql.Synthesis.Compilation.LINQ.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace Aidan.Webql.Synthesis.Compilation.LINQ;

/// <summary>
/// Provides a mechanism for translating WebQL query nodes into LINQ expressions. <br/>
/// This class serves as a central component for converting various query operations <br/>
/// such as filtering, projection, and aggregation into corresponding LINQ expressions.
/// </summary>
public abstract class LinqProvider : ILinqProvider
{
    public static LinqProvider Queryable => new QueryableLinqProvider();

    public static LinqProvider AsyncQueryable => new AsyncQueryableLinqProvider();

    //*
    // LINQ 'Where' method. 
    //*

    public MethodInfo GetWhereMethodInfo(TranslationContext context, ArgumentSemantic semantics)
    {
        switch (semantics.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableWhereMethodInfo()
                    .MakeGenericMethod(semantics.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableWhereMethodInfo()
                     .MakeGenericMethod(semantics.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected abstract MethodInfo GetQueryableWhereMethodInfo();

    protected MethodInfo GetEnumerableWhereMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters()
            })
            .Where(x => x.Params.Length == 2
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
            .Select(x => x.Method)
            .First(m => m != null);
    }

    //*
    // LINQ 'Select' method. 
    //*

    public MethodInfo GetSelectMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableSelectMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context), resultType);

            case LinqSourceType.Enumerable:
                return GetEnumerableSelectMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context), resultType);

            default:
                throw new InvalidOperationException();
        }
    }

    protected abstract MethodInfo GetQueryableSelectMethodInfo();

    protected MethodInfo GetEnumerableSelectMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length == 2
                && x.Args.Length == 2
                && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
            .Select(x => x.Method)
            .First(m => m != null);
    }

    //*
    // LINQ 'SelectMany' method. 
    //*

    public MethodInfo GetSelectManyMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableSelectManyMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context), resultType);

            case LinqSourceType.Enumerable:
                return GetEnumerableSelectManyMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context), resultType);

            default:
                throw new InvalidOperationException();
        }
    }

    protected abstract MethodInfo GetQueryableSelectManyMethodInfo();

    protected MethodInfo GetEnumerableSelectManyMethodInfo()
    {

        return typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(x => new
            {
                Name = x.Name,
                Parameters = x.GetParameters(),
                GenericArguments = x.GetGenericArguments(),
                MethodInfo = x
            })
            .Where(x => x.Name == "SelectMany")
            .Where(x => x.Parameters.Length == 2)
            .Where(x => x.GenericArguments.Length == 2)
            .Select(x => x.MethodInfo)
            .First();

        return typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "SelectMany"
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 2
                && m.GetParameters().Length == 2
                && m.GetParameters()[0].ParameterType.IsGenericType
                && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && m.GetParameters()[1].ParameterType.IsGenericType
                && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType
                && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>));
    }

    //*
    // LINQ 'Take' method. 
    //*

    public MethodInfo GetTakeMethodInfo(TranslationContextOld context, QueryArgumentExpression source)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableTakeMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableTakeMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected abstract MethodInfo GetQueryableTakeMethodInfo();

    protected MethodInfo GetEnumerableTakeMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Take" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                m.GetParameters()[1].ParameterType == typeof(int));
    }

    //*
    // LINQ 'Skip' method. 
    //*

    public MethodInfo GetSkipMethodInfo(TranslationContextOld context, QueryArgumentExpression source)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableSkipMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableSkipMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected abstract MethodInfo GetQueryableSkipMethodInfo();

    protected MethodInfo GetEnumerableSkipMethodInfo()
    {
        return typeof(Enumerable)
           .GetMethods()
           .First(m => m.Name == "Skip" &&
               m.IsGenericMethodDefinition &&
               m.GetParameters().Length == 2 &&
               m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
               m.GetParameters()[1].ParameterType == typeof(int));
    }

    //*
    // LINQ 'Any' method. 
    //*

    public MethodInfo GetAnyMethodInfo(TranslationContextOld context, QueryArgumentExpression source)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableAnyMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableAnyMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected MethodInfo GetQueryableAnyMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    protected MethodInfo GetEnumerableAnyMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool)
            );
    }

    //*
    // LINQ 'All' method. 
    //*

    public MethodInfo GetAllMethodInfo(TranslationContextOld context, QueryArgumentExpression source)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableAllMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableAllMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected MethodInfo GetQueryableAllMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "All" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
        );
    }

    protected MethodInfo GetEnumerableAllMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "All" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[1] == typeof(bool)
        );
    }

    //*
    // LINQ 'Count' method. 
    //*

    public MethodInfo GetCountMethodInfo(TranslationContextOld context, QueryArgumentExpression source)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableCountMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableCountMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected MethodInfo GetQueryableCountMethodInfo()
    {
        return typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == "Count" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
            );
    }

    protected MethodInfo GetEnumerableCountMethodInfo()
    {
        return typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Count" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            );
    }


    //*
    // LINQ 'Min' method. 
    //*

    public MethodInfo GetMinMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableMinMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableMinMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected MethodInfo GetQueryableMinMethodInfo()
    {
        return typeof(Queryable).GetMethods()
            .First(m => m.Name == "Min" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));
    }

    protected MethodInfo GetEnumerableMinMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Min" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }


    //*
    // LINQ 'Max' method. 
    //*

    public MethodInfo GetMaxMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType)
    {
        switch (source.GetLinqSourceType(context))
        {
            case LinqSourceType.Queryable:
                return GetQueryableMaxMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            case LinqSourceType.Enumerable:
                return GetEnumerableMaxMethodInfo()
                    .MakeGenericMethod(source.GetElementType(context));

            default:
                throw new InvalidOperationException();
        }
    }

    protected MethodInfo GetQueryableMaxMethodInfo()
    {
        return typeof(Queryable).GetMethods()
           .First(m => m.Name == "Max" &&
               m.IsGenericMethodDefinition &&
               m.GetParameters().Length == 1 &&
               m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));
    }

    protected MethodInfo GetEnumerableMaxMethodInfo()
    {
        return typeof(Enumerable).GetMethods()
           .First(m => m.Name == "Max" &&
               m.IsGenericMethodDefinition &&
               m.GetParameters().Length == 1 &&
               m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    //*
    //*
    // Unfinished section. From this point down is all todo code.
    //*
    //*

    //*
    // LINQ 'Sum' method. 
    //*

    protected virtual MethodInfo FindSumMethod(Type returnType)
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Sum" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.IsGenericType &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType);
    }

    protected virtual MethodInfo GetIntSumMethodInfo()
    {
        return FindSumMethod(typeof(int));
    }

    protected virtual MethodInfo GetInt64SumMethodInfo()
    {
        return FindSumMethod(typeof(long));
    }

    protected virtual MethodInfo GetFloatSumMethodInfo()
    {
        return FindSumMethod(typeof(float));
    }

    protected virtual MethodInfo GetDoubleSumMethodInfo()
    {
        return FindSumMethod(typeof(double));
    }

    protected virtual MethodInfo GetDecimalSumMethodInfo()
    {
        return FindSumMethod(typeof(decimal));
    }

    //*
    // LINQ 'Average' method. 
    //*

    protected virtual MethodInfo FindAverageMethod(Type returnType)
    {
        return typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Average" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.IsGenericType &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                m.GetParameters()[1].ParameterType.IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].IsGenericType &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>) &&
                m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments()[1] == returnType);
    }

    protected virtual MethodInfo GetIntAverageMethodInfo()
    {
        return FindAverageMethod(typeof(int));
    }

    protected virtual MethodInfo GetInt64AverageMethodInfo()
    {
        return FindAverageMethod(typeof(long));
    }

    protected virtual MethodInfo GetFloatAverageMethodInfo()
    {
        return FindAverageMethod(typeof(float));
    }

    protected virtual MethodInfo GetDoubleAverageMethodInfo()
    {
        return FindAverageMethod(typeof(double));
    }

    protected virtual MethodInfo GetDecimalAverageMethodInfo()
    {
        return FindAverageMethod(typeof(decimal));
    }

}
