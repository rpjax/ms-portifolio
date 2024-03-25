using ModularSystem.Core;
using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;
using ModularSystem.Webql.Synthesis.Compilation.LINQ;
using System.Collections;

namespace ModularSystem.Webql;

public static class WebqlHelper
{
    public static bool TypeIsEnumerable(TranslationContextOld context, Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        // Verifica se o tipo é genérico e implementa IEnumerable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return true;
        }

        // Verifica se algum dos tipos de interface implementados é um IEnumerable<T>
        foreach (Type interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }
        }

        return false;
    }

    public static bool TypeIsNotEnumerable(TranslationContextOld context, Type type)
    {
        return !TypeIsEnumerable(context, type);
    }

    public static bool TypeIsQueryable(TranslationContextOld context, Type type)
    {
        return
           typeof(IEnumerable).IsAssignableFrom(type)
           || type.GetInterfaces().Any(i =>
              i.IsGenericType &&
              i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool TypeIsNotQueryable(TranslationContextOld context, Type type)
    {
        return !TypeIsQueryable(context, type);
    }

    public static Type? TryGetElementType(TranslationContextOld context, Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var queryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .FirstOrDefault();

        if (queryableInterface != null)
        {
            return queryableInterface.GetGenericArguments()[0];
        }

        var enumerableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .FirstOrDefault();

        if (enumerableInterface != null)
        {
            return enumerableInterface.GetGenericArguments()[0];
        }

        return null;
    }

    public static Type GetElementType(TranslationContextOld context, Type type)
    {
        return TryGetElementType(context, type) 
            ?? throw SemanticThrowHelper.ErrorInternalUnknown(context, "The current context does not represent a queryable type or the queryable type is undefined. Ensure that the context is correctly initialized and represents a valid queryable type. This error may indicate a misalignment between the expected and actual types within the context.");
    }

    public static LinqSourceType GetLinqSourceType(TranslationContextOld context, Type type)
    {
        if (TypeIsQueryable(context, type))
        {
            return LinqSourceType.Queryable;
        }
        if (TypeIsEnumerable(context, type))
        {
            return LinqSourceType.Enumerable;
        }

        var message = $"The type '{type.FullName}' is not supported for LINQ operations because it does not implement either IQueryable<T> or IEnumerable<T>.";

        throw new TranslationException(message, context);
    }

    /// <summary>
    /// Converts an <see cref="Operator"/> enum value into a string representation.
    /// </summary>
    /// <param name="op">The OperatorV2 enum value.</param>
    /// <returns>The string representation of the operator.</returns>
    public static string Stringify(Operator op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    /// <summary>
    /// Converts a string representation of an operator into its corresponding <see cref="Operator"/> enum value.
    /// </summary>
    /// <param name="value">The string representation of the operator.</param>
    /// <returns>The OperatorV2 enum value.</returns>
    /// <exception cref="GeneratorException">Thrown when the operator string is not recognized.</exception>
    public static Operator? TryParseOperatorString(string value)
    {
        var operators = Enum.GetValues(typeof(Operator));

        foreach (Operator op in operators)
        {
            if (Stringify(op) == value)
            {
                return op.TypeCast<Operator>();
            }
        }

        return null;
    }

    public static Operator ParseOperatorString(TranslationContextOld context, string value)
    {
        return TryParseOperatorString(value)
            ?? throw new TranslationException("", context);
    }

    public static OperatorType GetOperatorType(Operator op)
    {
        switch (op)
        {
            case Operator.Add:
            case Operator.Subtract:
            case Operator.Divide:
            case Operator.Multiply:
            case Operator.Modulo:
                return OperatorType.Arithmetic;

            case Operator.Equals:
            case Operator.NotEquals:
            case Operator.Less:
            case Operator.LessEquals:
            case Operator.Greater:
            case Operator.GreaterEquals:
                return OperatorType.Relational;

            case Operator.Like:
            case Operator.RegexMatch:
                return OperatorType.PatternRelational;

            case Operator.Or:
            case Operator.And:
            case Operator.Not:
                return OperatorType.Logical;

            case Operator.Expr:
            case Operator.Literal:
                return OperatorType.Semantic;

            case Operator.Select:
            case Operator.Filter:
            case Operator.Transform:
            case Operator.SelectMany:
            case Operator.Project:
            case Operator.Limit:
            case Operator.Skip:
            case Operator.Any:
            case Operator.All:
                return OperatorType.Queryable;

            case Operator.Count:
            case Operator.Index:
            case Operator.Min:
            case Operator.Max:
            case Operator.Sum:
            case Operator.Average:
                return OperatorType.Aggregation;

            default:
                throw new InvalidOperationException();
        }
    }

    public static OperatorCategory GetOperatorCategory(Operator @operator)
    {
        switch (@operator)
        {
            //*
            // Unary Operators.
            //*
            case Operator.Not:
            case Operator.Limit:
            case Operator.Skip:
            case Operator.Or:
            case Operator.And:
            case Operator.Expr:
            case Operator.Literal:

                return OperatorCategory.Unary;

            //*
            // Binary Operators.
            //*

            case Operator.Add:
            case Operator.Subtract:
            case Operator.Divide:
            case Operator.Multiply:
            case Operator.Modulo:
            case Operator.Equals:
            case Operator.NotEquals:
            case Operator.Less:
            case Operator.LessEquals:
            case Operator.Greater:
            case Operator.GreaterEquals:
            case Operator.Like:
            case Operator.RegexMatch:
            case Operator.Select:
            case Operator.Filter:
            case Operator.Project:
            case Operator.Transform:
            case Operator.Index:
            case Operator.Any:
            case Operator.All:
            case Operator.Min:
            case Operator.Max:
            case Operator.Sum:
            case Operator.Average:
            case Operator.SelectMany:
            case Operator.Count:

                return OperatorCategory.Ternary;

            //*
            // Ternary Operators.
            //*

            // there is no ternary operator...

            default:
                throw new InvalidOperationException();
        }
    }

    public static bool OperatorEvaluatesToBool(Operator @operator)
    {
        var opType = GetOperatorType(@operator);

        return
            opType == OperatorType.Relational
            || opType == OperatorType.PatternRelational
            || opType == OperatorType.Logical;
    }

}

