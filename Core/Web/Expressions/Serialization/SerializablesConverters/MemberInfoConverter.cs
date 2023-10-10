using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <inheritdoc/>
public interface IMemberInfoConverter : IConverter<MemberInfo, SerializableMemberInfo>
{

}

/// <inheritdoc/>
public class MemberInfoConverter : Converter,  IMemberInfoConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the configuration settings for the converter.
    /// </summary>
    private Configs Config { get; }

    private ITypeConverter TypeConverter => Config.TypeConverter;

    public MemberInfoConverter(ParsingContext context, Configs config)
    {
        Context = context;
        Config = config;
    }

    /// <inheritdoc/>
    public SerializableMemberInfo Convert(MemberInfo instance)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(instance.DeclaringType!),
            Name = instance.Name,
        };
    }

    /// <inheritdoc/>
    public MemberInfo Convert(SerializableMemberInfo sMemberInfo)
    {
        if (sMemberInfo.DeclaringType == null)
        {
            throw MissingArgumentException(nameof(sMemberInfo.DeclaringType));
        }
        if (sMemberInfo.Name == null)
        {
            throw MissingArgumentException(nameof(sMemberInfo.Name));
        }

        var type = TypeConverter.Convert(sMemberInfo.DeclaringType);
        var memberInfo = type.GetMember(sMemberInfo.Name);

        if (memberInfo.IsEmpty())
        {

        }
        if (memberInfo.Length > 1)
        {

        }

        return memberInfo.First();
    }

    /// <summary>
    /// Represents configuration settings for the <see cref="ElementInitConverter"/>.
    /// </summary>
    public class Configs
    {
        public ITypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            TypeConverter ??= dependencyContainer.GetInterface<ITypeConverter>();
        }
    }
}
