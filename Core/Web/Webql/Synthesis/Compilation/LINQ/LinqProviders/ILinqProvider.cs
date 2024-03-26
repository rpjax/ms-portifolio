using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Synthesis.Symbols;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

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
    MethodInfo GetWhereMethodInfo(TranslationContext context, ArgumentSemantic semantics);
    MethodInfo GetSelectMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType);
    MethodInfo GetTakeMethodInfo(TranslationContextOld context, QueryArgumentExpression source);
    MethodInfo GetSkipMethodInfo(TranslationContextOld context, QueryArgumentExpression source);

    //*
    // Aggregation LINQ methods. (returns the aggregation result of type <TResult>, it materializes the query)
    //*
    MethodInfo GetAnyMethodInfo(TranslationContextOld context, QueryArgumentExpression source);
    MethodInfo GetAllMethodInfo(TranslationContextOld context, QueryArgumentExpression source);
    MethodInfo GetCountMethodInfo(TranslationContextOld context, QueryArgumentExpression source);
    MethodInfo GetMinMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType);
    MethodInfo GetMaxMethodInfo(TranslationContextOld context, QueryArgumentExpression source, Type resultType);

}