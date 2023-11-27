using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql;

public class GeneratorContext
{
    public Type Type { get; }
    public Expression Expression { get; }
    public GeneratorContext? ParentContext { get; }
    public string Stack { get; }

    public GeneratorContext(Type type, Expression expression, GeneratorContext? parentContext = null, string stack = null)
    {
        Type = type;
        Expression = expression;
        ParentContext = parentContext;
        Stack = stack ?? "$root";
    }

    public PropertyInfo? GetPropertyInfo(string name)
    {
        var propertyInfo = Type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
            .FirstOrDefault();

        return propertyInfo;
    }

    public GeneratorContext CreateSubContext(string propertyName)
    {
        var propertyInfo = GetPropertyInfo(propertyName);

        if (propertyInfo == null)
        {
            throw new GeneratorException($"Property '{propertyName}' not found in the type '{Type.FullName}'. Ensure the property name is correct and exists in the specified type.", null);
        }

        var subType = propertyInfo.PropertyType;
        var expression = Expression.MakeMemberAccess(Expression, propertyInfo);
        var subStack = $"{Stack}->{propertyName}";

        return new(subType, expression, this, subStack);
    }

}