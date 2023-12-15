using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public class ConversionContext
{
    public IExpressionConverter ExpressionConverter { get; }
    public ITypeConverter TypeConverter { get; }
    public IParameterInfoConverter ParameterInfoConverter { get; }
    public IMemberInfoConverter MemberInfoConverter { get; }
    public IPropertyInfoConverter PropertyInfoConverter { get; }
    public IMethodInfoConverter MethodInfoConverter { get; }
    public IConstructorInfoConverter ConstructorInfoConverter { get; }
    public IMemberBindingConverter MemberBindingConverter { get; }
    public IElementInitConverter ElementInitConverter { get; }
    public ISerializer Serializer { get; }

    private ConversionContext? Parent { get; }
    private ReferenceTable ReferenceTable { get; }

    public ConversionContext(ConversionContext? parent = null)
    {
        ExpressionConverter = CreateExpressionConverter();
        TypeConverter = CreateTypeConverter();
        ParameterInfoConverter = CreateParameterInfoConverter();
        MemberInfoConverter = CreateMemberInfoConverter();
        PropertyInfoConverter = CreatePropertyInfoConverter();
        MethodInfoConverter = CreateMethodInfoConverter();
        ConstructorInfoConverter = CreateConstructorInfoConverter();
        MemberBindingConverter = CreateMemberBindingConverter();
        ElementInitConverter = CreateElementInitConverter();
        Serializer = CreateSerializer();
        Parent = parent;
    }

    public  ConversionContext CreateChild()
    {
        return new ConversionContext(this);
    }

    private Dictionary<string, Expression> RefTable { get; } = new();
    private int Counter { get; set; } = 0;

    public string GetExpressionReferenceString(Expression expression)
    {
        var hash = expression.GetHashCode().ToString();

        if(RefTable.TryGetValue(hash, out var value))
        {
            return
        }

        return "";
    }

    protected virtual IExpressionConverter CreateExpressionConverter()
    {
        return new ExpressionConverter(this);
    }

    protected virtual ITypeConverter CreateTypeConverter()
    {
        return new TypeConverter(this, Expressions.TypeConverter.TypeConversionStrategy.UseAssemblyName);
    }

    protected virtual IParameterInfoConverter CreateParameterInfoConverter()
    {
        return new ParameterInfoConverter(this);
    }

    protected virtual IMemberInfoConverter CreateMemberInfoConverter()
    {
        return new MemberInfoConverter(this);
    }

    protected virtual IPropertyInfoConverter CreatePropertyInfoConverter()
    {
        return new PropertyInfoConverter(this);
    }

    protected virtual IMethodInfoConverter CreateMethodInfoConverter()
    {
        return new MethodInfoConverter(this);
    }

    protected virtual IConstructorInfoConverter CreateConstructorInfoConverter()
    {
        return new ConstructorInfoConverter(this);
    }

    protected virtual IMemberBindingConverter CreateMemberBindingConverter()
    {
        return new MemberBindingConverter(this);
    }

    protected virtual IElementInitConverter CreateElementInitConverter()
    {
        return new ElementInitConverter(this);
    }

    protected virtual ISerializer CreateSerializer()
    {
        return new ExprJsonSerializer();
    }

}

public class ReferenceTable
{
    private int Counter { get; set; } = 0;
    private Dictionary<string, Expression> Table { get; } = new();

    public string CreateKey()
    {

    }
}

public class ReferenceRecord
{
    public string Hash { get; set; }
    public Expression Expression { get; set; }
}