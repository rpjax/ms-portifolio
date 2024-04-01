using System.Numerics;

namespace ModularSystem.Webql.Analysis.Semantics;

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

}
