using ModularSystem.Webql.Synthesis.Symbols;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

//*
// LINQ methods providers.
//*

/// <summary>
/// Provides method info builders for translation.
/// </summary>
public interface ILinqProvider
{
    //*
    // Queryable LINQ methods. (returns the queryable itself, without materializing it)
    //*
    MethodInfo GetWhereMethodInfo(TranslationContext context, QueryArgumentExpression source);
    MethodInfo GetSelectMethodInfo(TranslationContext context, QueryArgumentExpression source, Type resultType);
    MethodInfo GetTakeMethodInfo(TranslationContext context, QueryArgumentExpression source);
    MethodInfo GetSkipMethodInfo(TranslationContext context, QueryArgumentExpression source);

    //*
    // Aggregation LINQ methods. (returns the aggregation result of type <TResult>, it materializes the query)
    //*
    MethodInfo GetAnyMethodInfo(TranslationContext context, QueryArgumentExpression source);
    MethodInfo GetAllMethodInfo(TranslationContext context, QueryArgumentExpression source);
    MethodInfo GetCountMethodInfo(TranslationContext context, QueryArgumentExpression source);
    MethodInfo GetMinMethodInfo(TranslationContext context, QueryArgumentExpression source, Type resultType);
    MethodInfo GetMaxMethodInfo(TranslationContext context, QueryArgumentExpression source, Type resultType);

}