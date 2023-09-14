namespace ModularSystem.Core;

public static class TypeExtensions
{
    public static bool IsSubclassOfRawGeneric(this Type source, Type targetType)
    {
        Type? _type = source;

        while (_type != null && _type != typeof(object))
        {
            var genericTypeConversion = _type.IsGenericType ? _type.GetGenericTypeDefinition() : _type;

            if (targetType == genericTypeConversion)
            {
                return true;
            }

            _type = _type.BaseType;
        }
        return false;
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

