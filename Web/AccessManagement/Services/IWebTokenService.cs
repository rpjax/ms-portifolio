using Aidan.Web.AccessManagement.Tokens;

namespace Aidan.Web.AccessManagement.Services;

/// <summary>
/// Represents a service for creating web tokens.
/// </summary>
public interface IWebTokenService
{
    /// <summary>
    /// Creates a web token based on the provided identity.
    /// </summary>
    /// <param name="identity">The identity used to create the token.</param>
    /// <returns>The created web token.</returns>
    IWebToken CreateToken(IIdentity identity);
}
