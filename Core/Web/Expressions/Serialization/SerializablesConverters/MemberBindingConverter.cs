using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="MemberMemberBinding"/> and its serializable counterpart, <see cref="SerializableMemberBinding"/>.
/// </summary>
public interface IMemberBindingConverter : IConverter<MemberMemberBinding, SerializableMemberBinding>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="MemberMemberBinding"/> and <see cref="SerializableMemberBinding"/>.
/// </summary>
public class MemberBindingConverter : Converter, IMemberBindingConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the configuration settings for the converter.
    /// </summary>
    private Configs Config { get; }

    /// <summary>
    /// Gets the member info converter used for member info conversions.
    /// </summary>
    private IMemberInfoConverter MemberInfoConverter => Config.MemberInfoConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberBindingConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="config">The configuration settings for the converter.</param>
    public MemberBindingConverter(ParsingContext context, Configs config)
    {
        Context = context;
        Config = config;
    }

    /// <summary>
    /// Converts a <see cref="MemberMemberBinding"/> to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The member binding to convert.</param>
    /// <returns>The serializable representation of the member binding.</returns>
    public SerializableMemberBinding Convert(MemberMemberBinding instance)
    {
        return new()
        {
            MemberInfo = MemberInfoConverter.Convert(instance.Member),
            Bindings = instance.Bindings
                .Where(x => x is MemberMemberBinding)
                .Select(x => (MemberMemberBinding)x)
                .Transform(x => Convert(x)).ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableMemberBinding"/> back to its original <see cref="MemberMemberBinding"/> form.
    /// </summary>
    /// <param name="sMemberBinding">The serializable member binding to convert.</param>
    /// <returns>The original member binding.</returns>
    public MemberMemberBinding Convert(SerializableMemberBinding sMemberBinding)
    {
        if (sMemberBinding.MemberInfo == null)
        {
            throw MissingArgumentException(nameof(sMemberBinding.MemberInfo));
        }
        if (sMemberBinding.Bindings == null)
        {
            throw MissingArgumentException(nameof(sMemberBinding.Bindings));
        }

        var memberInfo = MemberInfoConverter.Convert(sMemberBinding.MemberInfo);
        var bindings = sMemberBinding.Bindings
            .Transform(x => Convert(x))
            .ToArray();

        return Expression.MemberBind(memberInfo, bindings);
    }

    /// <summary>
    /// Represents configuration settings for the <see cref="MemberBindingConverter"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets the member info converter used for member info conversions.
        /// </summary>
        public IMemberInfoConverter MemberInfoConverter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            MemberInfoConverter ??= dependencyContainer.GetInterface<IMemberInfoConverter>();
        }
    }
}
