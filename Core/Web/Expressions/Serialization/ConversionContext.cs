using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

public abstract class ConversionContext
{
    public string[] Stack { get; }

    public ConversionContext(string label, string[]? stack = null)
    {
        Stack = stack != null 
            ? new List<string>(stack).FluentAdd(label).ToArray() 
            : new[] { label }; 
    }

    public override string ToString()
    {
        return string.Join("->", Stack);
    }

    public abstract ConversionContext CreateChild(string label);

    public abstract T GetDependency<T>();
    
}

public class DefaultConversionContext : ConversionContext
{
    private DependencyContainerObject DependencyContainer { get; init; }

    public DefaultConversionContext(string label, string[]? stack = null) : base(label, stack)
    {
        DependencyContainer = new();
        SetFactories();
    }

    private DefaultConversionContext(DependencyContainerObject dependencyContainer,string label, string[]? stack = null) : this(label, stack)
    {
        DependencyContainer = dependencyContainer;
    }

    public override ConversionContext CreateChild(string label)
    {
        return new DefaultConversionContext(DependencyContainer, label, Stack);
    }

    public override T GetDependency<T>()
    {
        var factory = DependencyContainer.TryGet<IStrategy<ConversionContext, T>>();

        if (factory == null)
        {
            throw new Exception($"Missing parsing dependency factory for type '{typeof(T).FullName}'.");
        }

        var dependency = factory.Execute(this);

        if (dependency == null)
        {
            throw new Exception($"The parsing factory failed do build a dependency of type '{typeof(T).FullName}'.");
        }

        return dependency;
    }

    public ConversionContext SetDependency<T>(IStrategy<ConversionContext, T> factory)
    {
        DependencyContainer.Register(factory);
        return this;
    }

    private void SetFactories()
    {
        SetDependency(new LambdaStrategy<IExpressionConverter>(x => CreateExpressionConverter(x)));
        SetDependency(new LambdaStrategy<ITypeConverter>(x => CreateTypeConverter(x)));
        SetDependency(new LambdaStrategy<IParameterInfoConverter>(x => CreateParameterInfoConverter(x)));
        SetDependency(new LambdaStrategy<IMemberInfoConverter>(x => CreateMemberInfoConverter(x)));
        SetDependency(new LambdaStrategy<IPropertyInfoConverter>(x => CreatePropertyInfoConverter(x)));
        SetDependency(new LambdaStrategy<IMethodInfoConverter>(x => CreateMethodInfoConverter(x)));
        SetDependency(new LambdaStrategy<IConstructorInfoConverter>(x => CreateConstructorInfoConverter(x)));
        SetDependency(new LambdaStrategy<IMemberBindingConverter>(x => GetMemberBindingConverter(x)));
        SetDependency(new LambdaStrategy<IElementInitConverter>(x => GetElementInitConverter(x)));
        SetDependency(new LambdaStrategy<ISerializer>(x => GetSerializer(x)));
    }

    protected virtual IExpressionConverter CreateExpressionConverter(ConversionContext context)
    {
        return new ExpressionConverter(context);
    }

    protected virtual ITypeConverter CreateTypeConverter(ConversionContext context)
    {
        return new TypeConverter(context, TypeConverter.TypeConversionStrategy.UseAssemblyName);
    }

    protected virtual IParameterInfoConverter CreateParameterInfoConverter(ConversionContext context)
    {
        return new ParameterInfoConverter(context);
    }

    protected virtual IMemberInfoConverter CreateMemberInfoConverter(ConversionContext context)
    {
        return new MemberInfoConverter(context);
    }

    protected virtual IPropertyInfoConverter CreatePropertyInfoConverter(ConversionContext context)
    {
        return new PropertyInfoConverter(context);
    }

    protected virtual IMethodInfoConverter CreateMethodInfoConverter(ConversionContext context)
    {
        return new MethodInfoConverter(context);
    }

    protected virtual IConstructorInfoConverter CreateConstructorInfoConverter(ConversionContext context)
    {
        return new ConstructorInfoConverter(context);
    }

    protected virtual IMemberBindingConverter GetMemberBindingConverter(ConversionContext context)
    {
        return new MemberBindingConverter(context);
    }

    protected virtual IElementInitConverter GetElementInitConverter(ConversionContext context)
    {
        return new ElementInitConverter(context);
    }

    protected virtual ISerializer GetSerializer(ConversionContext context)
    {
        return new ExprJsonSerializer();
    }

}

internal class LambdaStrategy<T> : IStrategy<ConversionContext, T>
{
    private readonly Func<ConversionContext, T> lambda;

    public LambdaStrategy(Func<ConversionContext, T> lambda)
    {
        this.lambda = lambda;
    }

    public T? Execute(ConversionContext? input)
    {
        if (input == null)
        {
            throw new InvalidOperationException();
        }

        return lambda.Invoke(input);
    }
}
