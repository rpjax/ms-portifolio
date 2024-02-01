namespace ModularSystem.Web.Attributes;

/// <summary>
/// Indicates that an action should be excluded from the permission mapping process. <br/>
/// When this attribute is applied, it bypasses the standard behavior that raises an exception for routes <br/>
/// without an associated <see cref="IdentityActionAttribute"/>.
/// </summary>
/// <remarks>
/// This attribute is useful for routes that are meant to be accessible without any specific permissions or identity action checks.
/// </remarks>
public class AnonymousActionAttribute : Attribute
{

}
