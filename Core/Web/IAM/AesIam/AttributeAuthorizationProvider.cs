using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Security;
using ModularSystem.Web.Attributes;

namespace ModularSystem.Web.Authorization;

/// <summary>
/// Provides an authorization implementation that uses the <see cref="IdentityActionAttribute"/> to determine <br/>
/// the corresponding <see cref="IdentityAction"/>, 
/// which then yields the appropriate <see cref="IResourcePolicy"/>.
/// </summary>
public class AttributeAuthorizationProvider : IAuthorizationProvider
{
    /// <summary>
    /// Gets or sets a value indicating whether action attributes without a corresponding <br/>
    /// identity action are permitted. When set to <c>true</c>, attributes without a corresponding action <br/>
    /// will not throw an error. Defaults to <c>false</c>.
    /// </summary>
    public bool AllowActionlessAttributes { get; set; }

    /// <summary>
    /// Represents the strategy for obtaining an <see cref="IdentityAction"/> based on the <see cref="IdentityActionAttribute"/>.
    /// </summary>
    private IAsyncStrategy<IdentityActionAttribute, IdentityAction> ActionGetterStrategy { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeAuthorizationProvider"/> class with the provided configuration options.
    /// </summary>
    /// <param name="options">The configuration options required for this provider.</param>
    public AttributeAuthorizationProvider(Options options)
    {
        AllowActionlessAttributes = options.AllowActionlessAttributes;
        ActionGetterStrategy = options.ActionGetterStrategy ?? throw new ArgumentNullException(nameof(options.ActionGetterStrategy));
    }

    ///<inheritdoc/>
    public async Task<IResourcePolicy?> GetResourcePolicyAsync(HttpContext httpContext)
    {
        var attribute = GetActionAttributeFrom(httpContext);

        if (attribute == null)
        {
            return null;
        }

        var action = await ActionGetterStrategy.ExecuteAsync(attribute);

        if (action == null)
        {
            if (!AllowActionlessAttributes)
            {
                throw new AppException($"The strategy failed to resolve an IdentityAction for the provided IdentityActionAttribute: {attribute.GetType().Name}.");
            }

            return null;
        }

        return action.GetResourcePolicy();
    }

    /// <summary>
    /// Extracts the <see cref="IdentityActionAttribute"/> from the provided HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context from which the attribute is extracted.</param>
    /// <returns>The extracted <see cref="IdentityActionAttribute"/> or null if it's absent.</returns>
    private IdentityActionAttribute? GetActionAttributeFrom(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            return null;
        }

        return endpoint.Metadata.GetMetadata<IdentityActionAttribute>();
    }

    /// <summary>
    /// Holds the configuration options for the <see cref="AttributeAuthorizationProvider"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets a value indicating whether action attributes without a corresponding <br/>
        /// identity action are permitted. When set to <c>true</c>, attributes without a corresponding action <br/>
        /// will not throw an error. Defaults to <c>false</c>.
        /// </summary>
        public bool AllowActionlessAttributes { get; set; }

        /// <summary>
        /// Defines the strategy used to retrieve an <see cref="IdentityAction"/> based on the <see cref="IdentityActionAttribute"/>.
        /// </summary>
        public IAsyncStrategy<IdentityActionAttribute, IdentityAction> ActionGetterStrategy { get; set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="Options"/> class using the provided strategy.
        /// </summary>
        /// <param name="actionGetterStrategy">The strategy used to extract an identity action based on its attribute.</param>
        public Options(IAsyncStrategy<IdentityActionAttribute, IdentityAction> actionGetterStrategy)
        {
            ActionGetterStrategy = actionGetterStrategy ?? throw new ArgumentNullException(nameof(actionGetterStrategy));
        }
    }
}
