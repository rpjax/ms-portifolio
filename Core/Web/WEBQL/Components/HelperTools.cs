using ModularSystem.Core;

namespace ModularSystem.Webql;

public static class HelperTools
{
    public static string Stringify(Operator op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    public static bool OperatorIsIterator(Operator op)
    {
        return op == Operator.Any || op == Operator.All;
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
    /// Converts an <see cref="OperatorV2"/> enum value into a string representation.
    /// </summary>
    /// <param name="op">The OperatorV2 enum value.</param>
    /// <returns>The string representation of the operator.</returns>
    public static string StringifyOperator(OperatorV2 op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    /// <summary>
    /// Converts a string representation of an operator into its corresponding <see cref="OperatorV2"/> enum value.
    /// </summary>
    /// <param name="value">The string representation of the operator.</param>
    /// <returns>The OperatorV2 enum value.</returns>
    /// <exception cref="GeneratorException">Thrown when the operator string is not recognized.</exception>
    public static OperatorV2 ParseOperatorString(string value)
    {
        var operators = Enum.GetValues(typeof(OperatorV2));

        foreach (OperatorV2 op in operators)
        {
            if (StringifyOperator(op) == value)
            {
                return op.TypeCast<OperatorV2>();
            }
        }

        throw new GeneratorException($"The operator '{value}' is not recognized or supported. Please ensure it is a valid operator.", null);
    }

    public static OperatorType GetOperatorType(OperatorV2 op)
    {
        switch (op)
        {
            case OperatorV2.Add:
            case OperatorV2.Subtract:
            case OperatorV2.Divide:
            case OperatorV2.Multiply:
            case OperatorV2.Modulo:
                return OperatorType.Arithmetic;

            case OperatorV2.Equals:
            case OperatorV2.NotEquals:
            case OperatorV2.Less:
            case OperatorV2.LessEquals:
            case OperatorV2.Greater:
            case OperatorV2.GreaterEquals:
                return OperatorType.Relational;

            case OperatorV2.Or:
            case OperatorV2.And:
            case OperatorV2.Not:
            case OperatorV2.Any:
            case OperatorV2.All:
                return OperatorType.Logical;

            case OperatorV2.Expr:
            case OperatorV2.Literal:
                return OperatorType.Semantic;

            case OperatorV2.Select:
            case OperatorV2.Filter:
            case OperatorV2.Project:
            case OperatorV2.Limit:
            case OperatorV2.Skip:
                return OperatorType.Queryable;

            case OperatorV2.Count:
            case OperatorV2.Index:
            case OperatorV2.Min:
            case OperatorV2.Max:
            case OperatorV2.Sum:
            case OperatorV2.Average:
                return OperatorType.Aggregation;

            default:
                throw new Exception();
        }
    }

}
