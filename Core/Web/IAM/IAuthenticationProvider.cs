using Microsoft.AspNetCore.Http;
using ModularSystem.Core.Security;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Provides methods to define the authentication strategy of the application.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Generates a token for the given identity.
    /// </summary>
    /// <param name="identity">The identity for which a token should be generated.</param>
    /// <returns>An <see cref="IToken"/> instance.</returns>
    IToken GetToken(IIdentity identity);

    /// <summary>
    /// Retrieves the token encrypter used by the authentication provider.
    /// </summary>
    /// <returns>An instance of <see cref="ITokenEncrypter"/> responsible for encryption and decryption of tokens.</returns>
    ITokenEncrypter GetTokenEncrypter();

    /// <summary>
    /// Retrieves the token associated with the given HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context for which the token should be retrieved.</param>
    /// <returns>An <see cref="IToken"/> instance if found; otherwise, null.</returns>
    IToken? GetToken(HttpContext httpContext);

    /// <summary>
    /// Retrieves the identity associated with the provided token.
    /// </summary>
    /// <param name="token">The token for which the identity should be retrieved.</param>
    /// <returns>An <see cref="IIdentity"/> instance if found; otherwise, null.</returns>
    IIdentity? GetIdentity(IToken token);
}

/// <summary>
/// Provides methods to define the authentication strategy of the application asynchronously.
/// </summary>
public interface IAsyncAuthenticationProvider
{
    /// <summary>
    /// Asynchronously generates a token for the given identity.
    /// </summary>
    /// <param name="identity">The identity for which a token should be generated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IToken"/> instance.</returns>
    Task<IToken> GetTokenAsync(IIdentity identity);

    /// <summary>
    /// Retrieves the token encrypter used by the authentication provider.
    /// </summary>
    /// <returns>An instance of <see cref="ITokenEncrypter"/> responsible for encryption and decryption of tokens.</returns>
    ITokenEncrypter GetTokenEncrypter();

    /// <summary>
    /// Asynchronously retrieves the token associated with the given HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context for which the token should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IToken"/> instance if found; otherwise, null.</returns>
    Task<IToken?> GetTokenAsync(HttpContext httpContext);

    /// <summary>
    /// Asynchronously retrieves the identity associated with the provided token.
    /// </summary>
    /// <param name="token">The token for which the identity should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IIdentity"/> instance if found; otherwise, null.</returns>
    Task<IIdentity?> GetIdentityAsync(IToken token);
}
