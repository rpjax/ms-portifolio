using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;
using ModularSystem.Web.Authentication;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web;

/// <summary>
/// Provides a concrete implementation of the Identity and Access Management (IAM) system that employs <br/>
/// AES (Advanced Encryption Standard) cryptographic techniques. <br/>
/// This class encapsulates both authentication and authorization operations <br/>
/// using AES for secure token management and attribute-based authorization.
/// </summary>
public class AesAccessManagementSystem : IAccessManagementService
{
    /// <summary>
    /// Gets the provider responsible for handling authentication processes using AES encryption.
    /// </summary>
    public IAuthenticationService AuthenticationService { get; }

    /// <summary>
    /// Gets the provider responsible for handling authorization processes based on attributes.
    /// </summary>
    public IAuthorizationService AuthorizationService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesAccessManagementSystem"/> class with the provided configuration options.
    /// </summary>
    /// <param name="options">Configuration parameters needed to initialize the AES-based IAM system.</param>
    public AesAccessManagementSystem(Options options)
    {
        AuthenticationService = new AesAuthenticationService(options.AuthenticationOptions);
        AuthorizationService = new AttributeAuthorizationService(options.AuthorizationStrategy);
    }

    /// <summary>
    /// Initializes a new instance of the AesIamSystem class with specified authentication and authorization providers.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider to be used. If null, a default AesAuthenticationProvider will be instantiated using the provided options.</param>
    /// <param name="authorizationProvider">The authorization provider to be used. If null, a default AttributeAuthorizationProvider will be instantiated using the provided options.</param>
    public AesAccessManagementSystem(IAuthenticationService authenticationProvider, IAuthorizationService authorizationProvider)
    {
        AuthenticationService = authenticationProvider;
        AuthorizationService = authorizationProvider;
    }

    /// <summary>
    /// Obtains the specific authentication provider in use that leverages AES cryptographic methods.
    /// </summary>
    /// <returns>The <see cref="AesAuthenticationService"/> instance managing authentication in the current IAM system.</returns>
    public AesAuthenticationService GetAuthenticationProvider()
    {
        return (AesAuthenticationService)AuthenticationService;
    }

    /// <summary>
    /// Retrieves the specific authorization provider in use that operates on attribute-driven policies.
    /// </summary>
    /// <returns>The <see cref="AttributeAuthorizationService"/> instance managing authorization in the current IAM system.</returns>
    public AttributeAuthorizationService GetAuthorizationProvider()
    {
        return (AttributeAuthorizationService)AuthorizationService;
    }

    /// <summary>
    /// Defines the configuration settings necessary for initializing the <see cref="AesAccessManagementSystem"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the settings associated with the AES-based authentication provider, particularly pertaining to token encryption and decryption.
        /// </summary>
        public AesAuthenticationService.Options? AuthenticationOptions { get; set; }

        /// <summary>
        /// Gets or sets the settings associated with the attribute-driven authorization provider.
        /// </summary>
        public IAsyncStrategy<string, AccessPolicy> AuthorizationStrategy { get; set; }

        public Options(
            AesAuthenticationService.Options? authenticationOptions,
            IAsyncStrategy<string, AccessPolicy> authorizationStrategy
        )
        {
            AuthenticationOptions = authenticationOptions;
            AuthorizationStrategy = authorizationStrategy;
        }

    }
}
