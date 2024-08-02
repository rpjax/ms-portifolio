namespace Aidan.Web.Attributes;

/// <summary>
/// Indicates that an action is explicitly forbidden regardless of any permissions or identity action checks. <br/>
/// When this attribute is applied, access to the associated route will always be denied.
/// </summary>
/// <remarks>
/// This attribute is useful for routes that should never be accessible, perhaps due to maintenance, deprecation, or other reasons.
/// </remarks>
public class ForbbidenActionAttribute : Attribute
{

}