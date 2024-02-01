using ModularSystem.Web.Authentication;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web;

/// <summary>
/// Provides a concrete implementation of the Identity and Access Management (IAM) system that employs <br/>
/// AES (Advanced Encryption Standard) cryptographic techniques. <br/>
/// This class encapsulates both authentication and authorization operations <br/>
/// using AES for secure token management and attribute-based authorization.
/// </summary>
public class AesIamSystem : IIamService
{
    /// <summary>
    /// Gets the provider responsible for handling authentication processes using AES encryption.
    /// </summary>
    public IAuthenticationProvider AuthenticationProvider { get; }

    /// <summary>
    /// Gets the provider responsible for handling authorization processes based on attributes.
    /// </summary>
    public IAuthorizationProvider AuthorizationProvider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesIamSystem"/> class with the provided configuration options.
    /// </summary>
    /// <param name="options">Configuration parameters needed to initialize the AES-based IAM system.</param>
    public AesIamSystem(Options options)
    {
        AuthenticationProvider = new AesAuthenticationProvider(options.AuthenticationOptions);
        AuthorizationProvider = new AttributeAuthorizationProvider(options.AuthorizationOptions);
    }

    /// <summary>
    /// Initializes a new instance of the AesIamSystem class with specified authentication and authorization providers.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider to be used. If null, a default AesAuthenticationProvider will be instantiated using the provided options.</param>
    /// <param name="authorizationProvider">The authorization provider to be used. If null, a default AttributeAuthorizationProvider will be instantiated using the provided options.</param>
    /// <param name="options">Options for configuring the authentication and authorization providers.</param>
    public AesIamSystem(IAuthenticationProvider? authenticationProvider, IAuthorizationProvider? authorizationProvider, Options options)
    {
        AuthenticationProvider = authenticationProvider ?? new AesAuthenticationProvider(options.AuthenticationOptions);
        AuthorizationProvider = authorizationProvider ?? new AttributeAuthorizationProvider(options.AuthorizationOptions);
    }

    /// <summary>
    /// Obtains the specific authentication provider in use that leverages AES cryptographic methods.
    /// </summary>
    /// <returns>The <see cref="AesAuthenticationProvider"/> instance managing authentication in the current IAM system.</returns>
    public AesAuthenticationProvider GetAuthenticationProvider()
    {
        return (AesAuthenticationProvider)AuthenticationProvider;
    }

    /// <summary>
    /// Retrieves the specific authorization provider in use that operates on attribute-driven policies.
    /// </summary>
    /// <returns>The <see cref="AttributeAuthorizationProvider"/> instance managing authorization in the current IAM system.</returns>
    public AttributeAuthorizationProvider GetAuthorizationProvider()
    {
        return (AttributeAuthorizationProvider)AuthorizationProvider;
    }

    /// <summary>
    /// Defines the configuration settings necessary for initializing the <see cref="AesIamSystem"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the settings associated with the AES-based authentication provider, particularly pertaining to token encryption and decryption.
        /// </summary>
        public AesAuthenticationProvider.Options AuthenticationOptions { get; set; }

        /// <summary>
        /// Gets or sets the settings associated with the attribute-driven authorization provider.
        /// </summary>
        public AttributeAuthorizationProvider.Options AuthorizationOptions { get; set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="Options"/> class with the given authentication and authorization settings.
        /// </summary>
        /// <param name="authenticationOptions">Settings specific to AES token management.</param>
        /// <param name="authorizationOptions">Settings specific to attribute-driven authorization.</param>
        public Options(AesAuthenticationProvider.Options? authenticationOptions = null, AttributeAuthorizationProvider.Options? authorizationOptions = null)
        {
            AuthenticationOptions = authenticationOptions ?? new();
            AuthorizationOptions = authorizationOptions ?? new();
        }
    }
}
