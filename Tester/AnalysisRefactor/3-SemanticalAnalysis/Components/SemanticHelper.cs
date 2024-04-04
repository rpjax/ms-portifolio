namespace ModularSystem.Webql.Analysis.Semantics.Helpers;

public static class SemanticHelper
{
    public static bool TypeIsNumber(this Type type)
    {
        return TypeIsIntNumber(type) || TypeIsFloatNumber(type);
    }

    public static bool TypeIsIntNumber(this Type type)
    {
        if (type == typeof(short) || type == typeof(ushort))
        {
            return true;
        }
        if (type == typeof(int) || type == typeof(uint))
        {
            return true;
        }
        if (type == typeof(long) || type == typeof(ulong))
        {
            return true;
        }

        return false;
    }

    public static bool TypeIsFloatNumber(this Type type)
    {
        if (type == typeof(float))
        {
            return true;
        }
        if (type == typeof(double))
        {
            return true;
        }
        if (type == typeof(decimal))
        {
            return true;
        }

        return false;
    }

    public static bool TypeIsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    public static Type GetNullableUnderlyingType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);

        if(underlyingType == null)
        {
            throw new InvalidOperationException();
        }

        return underlyingType;
    }

}
