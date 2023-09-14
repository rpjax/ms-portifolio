namespace ModularSystem.Web.Authentication;

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
        AuthenticationProvider = new AesTokenAuthenticationProvider(options?.AuthenticationOptions);
        AuthorizationProvider = new DefaultAuthorizationProvider();
    }

    /// <summary>
    /// Retrieves the current authentication provider that utilizes AES cryptography.
    /// </summary>
    /// <returns>The <see cref="AesTokenAuthenticationProvider"/> used in this IAM system.</returns>
    public AesTokenAuthenticationProvider GetAuthenticationProvider()
    {
        return (AesTokenAuthenticationProvider)AuthenticationProvider;
    }

    /// <summary>
    /// Represents the configuration options for the <see cref="AesIamSystem"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the authentication provider options specific to AES token encryption and decryption.
        /// </summary>
        public AesTokenAuthenticationProvider.Options? AuthenticationOptions { get; set; } = null;
    }
}
