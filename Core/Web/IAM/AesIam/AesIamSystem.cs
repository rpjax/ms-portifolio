using ModularSystem.Web.Authentication;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web;

/// <summary>
/// Provides an implementation of the Identity and Access Management (IAM) system that utilizes AES cryptography.
/// </summary>
public class AesIamSystem : IIamSystem
{
    /// <summary>
    /// Gets the provider used for authentication operations.
    /// </summary>
    public IAuthenticationProvider AuthenticationProvider { get; }

    /// <summary>
    /// Gets the provider used for authorization operations.
    /// </summary>
    public IAuthorizationProvider AuthorizationProvider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesIamSystem"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the IAM system.</param>
    public AesIamSystem(Options? options = null)
    {
        AuthenticationProvider = new AesAuthenticationProvider(options?.AuthenticationOptions);
        AuthorizationProvider = new AesAuthorizationProvider();
    }

    /// <summary>
    /// Retrieves the current authentication provider that utilizes AES cryptography.
    /// </summary>
    /// <returns>The <see cref="AesAuthenticationProvider"/> used in this IAM system.</returns>
    public AesAuthenticationProvider GetAuthenticationProvider()
    {
        return (AesAuthenticationProvider)AuthenticationProvider;
    }

    /// <summary>
    /// Retrieves the current authorization provider that utilizes AES cryptography.
    /// </summary>
    /// <returns>The <see cref="AesAuthorizationProvider"/> used in this IAM system.</returns>
    public AesAuthorizationProvider GetAuthorizationProvider()
    {
        return (AesAuthorizationProvider) AuthorizationProvider;
    }

    /// <summary>
    /// Represents the configuration options for the <see cref="AesIamSystem"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the authentication provider options specific to AES token encryption and decryption.
        /// </summary>
        public AesAuthenticationProvider.Options? AuthenticationOptions { get; set; } = null;
    }
}
