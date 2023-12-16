using ModularSystem.Core;

namespace ModularSystem.Webql;

public static class HelperTools
{
    public static string Stringify(OperatorOld op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    public static bool OperatorIsIterator(OperatorOld op)
    {
        return op == OperatorOld.Any || op == OperatorOld.All;
    }

    public static bool TypeIsEnumerable(Type type)
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

    public static Type GetEnumerableType(Type type)
    {
        // Verifica se o tipo é um IEnumerable<T> diretamente
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        // Verifica entre as interfaces implementadas
        var enumerableInterface = type
            .GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .FirstOrDefault();

        if (enumerableInterface == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return enumerableInterface.GetGenericArguments()[0];
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
    public static Operator ParseOperatorString(string value)
    {
        var operators = Enum.GetValues(typeof(Operator));

        foreach (Operator op in operators)
        {
            if (Stringify(op) == value)
            {
                return op.TypeCast<Operator>();
            }
        }

        throw new Exception($"The operator '{value}' is not recognized or supported. Please ensure it is a valid operator.");
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
            case Operator.Any:
            case Operator.All:
                return OperatorType.Logical;

            case Operator.Expr:
            case Operator.Literal:
                return OperatorType.Semantic;

            case Operator.Select:
            case Operator.Filter:
            case Operator.Project:
            case Operator.Limit:
            case Operator.Skip:
                return OperatorType.Queryable;

            case Operator.Count:
            case Operator.Index:
            case Operator.Min:
            case Operator.Max:
            case Operator.Sum:
            case Operator.Average:
                return OperatorType.Aggregation;

            default:
                throw new Exception();
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

