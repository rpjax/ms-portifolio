namespace ModularSystem.Core;

public static class TypeExtensions
{
    public static bool IsSubtypeOf(this Type childType, Type parentType)
    {
        Type? iteratorType = childType;

        while (iteratorType != null && iteratorType != typeof(object))
        {
            Type type;

            //*
            // this ensures that List<int> is not equal to List<string>.
            //*
            if (iteratorType.IsGenericType && parentType.IsGenericType)
            {
                type = iteratorType.GetGenericTypeDefinition();
            }
            else
            {
                type = iteratorType;
            }

            if (parentType == type)
            {
                return true;
            }

            iteratorType = iteratorType.BaseType;
        }

        return false;
    }

    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        foreach (var implementedInterface in type.GetInterfaces())
        {
            if (implementedInterface == interfaceType ||
                (interfaceType.IsGenericType && implementedInterface.IsGenericType &&
                implementedInterface.GetGenericTypeDefinition() == interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsSubtypeOf<T>(this Type childType)
    {
        return IsSubtypeOf(childType, typeof(T));
    }

    public static bool IsInstanceOfGenericType(this Type source, Type target)
    {
        Type? _type = source;

        while (_type != null && _type != typeof(object))
        {
            if (_type.IsGenericType && _type.GetGenericTypeDefinition() == target)
                return true;

            _type = _type.BaseType;
        }

        return false;
    }

    public static bool IsInstanceOfGenericType<T>(this Type source)
    {
        return source.IsInstanceOfGenericType(typeof(T));
    }

    public static int? InheritanceDistance(this Type baseType, Type derivedType)
    {
        int distance = 0;

        Type? targetType = derivedType;

        while (targetType != null && targetType != baseType)
        {
            targetType = targetType.BaseType;
            distance++;
        }

        if (derivedType == baseType)
        {
            return distance;
        }

        return null; // means derivedType is not derived from baseType
    }

    public static bool IsNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }
}

