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

    public static Type EvaluateOperatorReturnType(OperatorV2 op)
    {
        switch (op)
        {
            case OperatorV2.Add:
                break;
            case OperatorV2.Subtract:
                break;
            case OperatorV2.Divide:
                break;
            case OperatorV2.Multiply:
                break;
            case OperatorV2.Modulo:
                break;
            case OperatorV2.Equals:
                break;
            case OperatorV2.NotEquals:
                break;
            case OperatorV2.Less:
                break;
            case OperatorV2.LessEquals:
                break;
            case OperatorV2.Greater:
                break;
            case OperatorV2.GreaterEquals:
                break;
            case OperatorV2.Or:
                break;
            case OperatorV2.And:
                break;
            case OperatorV2.Not:
                break;
            case OperatorV2.Expr:
                break;
            case OperatorV2.Literal:
                break;
            case OperatorV2.Select:
                break;
            case OperatorV2.Filter:
                break;
            case OperatorV2.Project:
                break;
            case OperatorV2.Limit:
                break;
            case OperatorV2.Skip:
                break;
            case OperatorV2.Count:
                break;
            case OperatorV2.Index:
                break;
            case OperatorV2.Any:
                break;
            case OperatorV2.All:
                break;
            case OperatorV2.Min:
                break;
            case OperatorV2.Max:
                break;
            case OperatorV2.Sum:
                break;
            case OperatorV2.Average:
                break;
        }
    }

}
