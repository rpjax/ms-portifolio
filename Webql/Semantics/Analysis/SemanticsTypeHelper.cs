using System.Reflection;
using Webql.Core;
using Webql.Core.Helpers;

namespace Webql.Semantics.Analysis;

public static class SemanticsTypeHelper
{
    public static Type NormalizeNullableType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return Nullable.GetUnderlyingType(type)!;

        return type;
    }

    public static bool TypesAreCompatible(Type lhs, Type rhs)
    {
        if (lhs == rhs)
            return true;

        if (Nullable.GetUnderlyingType(lhs) == rhs || Nullable.GetUnderlyingType(rhs) == lhs)
            return true;

        lhs = NormalizeNullableType(lhs);
        rhs = NormalizeNullableType(rhs);

        if (HasImplicitConversion(lhs, rhs) || HasImplicitConversion(rhs, lhs))
            return true;

        return false;
    }

    public static bool HasImplicitConversion(Type fromType, Type toType)
    {
        if (fromType == null || toType == null)
            throw new ArgumentNullException("fromType or toType cannot be null");

        // Check for implicit conversion operators defined on the source type
        var sourceTypeMethods = fromType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == toType)
            .ToArray()
            ;

        if (sourceTypeMethods.Any())
            return true;

        // Check for implicit conversion operators defined on the target type
        var targetTypeMethods = toType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.GetParameters().FirstOrDefault()?.ParameterType == fromType);

        if (targetTypeMethods.Any())
            return true;

        // Check for direct assignment compatibility (e.g., int to decimal)
        if (toType.IsAssignableFrom(fromType))
            return true;

        return false;
    }

    public static PropertyInfo? TryGetPropertyFromType(Type type, string propertyName)
    {
        var normalizedName = IdentifierHelper.NormalizeIdentifier(propertyName);

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == "Length" || !x.DeclaringType?.Namespace?.StartsWith("System") == true)
            .FirstOrDefault(p => IdentifierHelper.NormalizeIdentifier(p.Name) == normalizedName);
    }

}

