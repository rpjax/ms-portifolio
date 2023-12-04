using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql;

public class Context
{
    public Type InputType { get; }
    public Expression InputExpression { get; }
    public Context? ParentContext { get; }

    public Context(Type inputType, Expression inputExpression, Context? parentContext = null)
    {
        InputType = inputType;
        InputExpression = inputExpression;
        ParentContext = parentContext;
    }

    public PropertyInfo? GetPropertyInfo(string name)
    {
        var propertyInfo = null as PropertyInfo;
        var context = this;

        while (true)
        {
            propertyInfo = InputType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name == name || x.Name.ToLower() == name.ToLower())
                .FirstOrDefault();

            if (propertyInfo != null)
            {
                break;
            }

            if (context.ParentContext == null)
            {
                break;
            }

            context = context.ParentContext;
        }

        if (propertyInfo == null)
        {
            throw new GeneratorException($"Property '{name}' not found in the type '{InputType.FullName}'. Ensure the property name is correct and exists in the specified type.", null);
        }

        return propertyInfo;
    }

    public Context AccessProperty(string propertyName, bool useParentContexts = true)
    {
        var propertyInfo = null as PropertyInfo;
        var context = this;

        while (true)
        {
            propertyInfo = context.GetPropertyInfo(propertyName);

            if (propertyInfo != null)
            {
                break;
            }

            if (context.ParentContext == null)
            {
                break;
            }

            if (!useParentContexts)
            {
                break;
            }

            context = context.ParentContext;
        }

        if (propertyInfo == null)
        {
            throw new GeneratorException($"Property '{propertyName}' not found in the type '{InputType.FullName}'. Ensure the property name is correct and exists in the specified type.", null);
        }
        if (context.InputExpression == null)
        {
            throw new Exception();
        }

        var subType = propertyInfo.PropertyType;
        var expression = Expression.MakeMemberAccess(context.InputExpression, propertyInfo);

        return new Context(subType, expression, this);
    }

    public bool IsVoid()
    {
        return InputType == typeof(void);
    }

    public bool IsQueryable()
    {
        return
            InputType.IsArray
            || InputType.IsGenericType
            && typeof(IEnumerable<>).IsAssignableFrom(InputType.GetGenericTypeDefinition());
    }

    public Type? GetQueryableType()
    {
        if (IsQueryable())
        {
            if (InputType!.IsArray)
            {
                return InputType.GetElementType();
            }
            else
            {
                return InputType.GetGenericArguments().FirstOrDefault();
            }
        }

        return null;
    }

    public ParameterExpression CreateParameterExpression()
    {
        return Expression.Parameter(InputType);
    }

    public string CreateParameterName()
    {
        return "x";
    }
}

