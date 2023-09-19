using ModularSystem.Core.Security;

namespace ModularSystem.Web.Attributes;

/// <summary>
/// Indicates that an action should be excluded from the Omega initializer's permission mapping process. <br/>
/// When this attribute is applied, it bypasses the standard behavior that raises an exception for routes <br/>
/// without an associated <see cref="IdentityActionAttribute"/>.
/// </summary>
/// <remarks>
/// This attribute is useful for routes that are meant to be accessible without any specific permissions or identity action checks.
/// </remarks>
public class AnonymousActionAttribute : Attribute
{
    /// <summary>
    /// Provides a string representation of the identity action in the "domain:resource:action" format.
    /// In the context of the <see cref="AnonymousActionAttribute"/>, this returns a default representation.
    /// </summary>
    /// <returns>A string representation of the identity action, defaulting to "domain:resource:action".</returns>
    public override string ToString()
    {
        return GetIdentityAction().ToString();
    }

    /// <summary>
    /// Returns an instance of <see cref="IdentityAction"/> with default values.
    /// </summary>
    /// <returns>The <see cref="IdentityAction"/> representation with default values.</returns>
    public IdentityAction GetIdentityAction()
    {
        return new IdentityAction(string.Empty, string.Empty, string.Empty);
    }
}
