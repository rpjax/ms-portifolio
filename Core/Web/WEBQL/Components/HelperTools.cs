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
}
