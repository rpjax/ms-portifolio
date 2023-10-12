using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

public abstract class ParsingContext
{
    public string[] Stack { get; }

    public ParsingContext(string label, string[]? stack = null)
    {
        Stack = stack != null 
            ? new List<string>(stack).FluentAdd(label).ToArray() 
            : new[] { label }; 
    }

    public override string ToString()
    {
        return string.Join("->", Stack);
    }

    public abstract ParsingContext CreateChild(string label);

    public abstract T GetDependency<T>();
    
}

public class DefaultParsingContext : ParsingContext
{
    private DependencyContainerObject DependencyContainer { get; init; }

    public DefaultParsingContext(string label, string[]? stack = null) : base(label, stack)
    {
        DependencyContainer = new();
        SetFactories();
    }

    private DefaultParsingContext(DependencyContainerObject dependencyContainer,string label, string[]? stack = null) : this(label, stack)
    {
        DependencyContainer = dependencyContainer;
    }

    public override ParsingContext CreateChild(string label)
    {
        return new DefaultParsingContext(DependencyContainer, label, Stack);
    }

    public override T GetDependency<T>()
    {
        var factory = DependencyContainer.TryGet<IStrategy<ParsingContext, T>>();

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

    public ParsingContext SetDependency<T>(IStrategy<ParsingContext, T> factory)
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

    protected virtual IExpressionConverter CreateExpressionConverter(ParsingContext context)
    {
        return new ExpressionConverter(context);
    }

    protected virtual ITypeConverter CreateTypeConverter(ParsingContext context)
    {
        return new TypeConverter(context, TypeConverter.TypeConversionStrategy.UseFullName);
    }

    protected virtual IParameterInfoConverter CreateParameterInfoConverter(ParsingContext context)
    {
        return new ParameterInfoConverter(context);
    }

    protected virtual IMemberInfoConverter CreateMemberInfoConverter(ParsingContext context)
    {
        return new MemberInfoConverter(context);
    }

    protected virtual IPropertyInfoConverter CreatePropertyInfoConverter(ParsingContext context)
    {
        return new PropertyInfoConverter(context);
    }

    protected virtual IMethodInfoConverter CreateMethodInfoConverter(ParsingContext context)
    {
        return new MethodInfoConverter(context);
    }

    protected virtual IConstructorInfoConverter CreateConstructorInfoConverter(ParsingContext context)
    {
        return new ConstructorInfoConverter(context);
    }

    protected virtual IMemberBindingConverter GetMemberBindingConverter(ParsingContext context)
    {
        return new MemberBindingConverter(context);
    }

    protected virtual IElementInitConverter GetElementInitConverter(ParsingContext context)
    {
        return new ElementInitConverter(context);
    }

    protected virtual ISerializer GetSerializer(ParsingContext context)
    {
        return new ExprToUtf8Serializer();
    }

}

internal class LambdaStrategy<T> : IStrategy<ParsingContext, T>
{
    private readonly Func<ParsingContext, T> lambda;

    public LambdaStrategy(Func<ParsingContext, T> lambda)
    {
        this.lambda = lambda;
    }

    public T? Execute(ParsingContext? input)
    {
        if (input == null)
        {
            throw new InvalidOperationException();
        }

        return lambda.Invoke(input);
    }
}
