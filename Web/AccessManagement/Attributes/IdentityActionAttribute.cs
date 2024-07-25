//using ModularSystem.Core.AccessManagement;

//namespace ModularSystem.Web.Attributes;

///// <summary>
///// Represents an attribute used to annotate MVC action methods with identity-related action information.
///// </summary>
///// <remarks>
///// The attribute provides a way to map a controller action method to a specific <see cref="ModularSystem.Core.AccessManagement.IdentityAction"/>, <br/>
///// which is used for access control purposes based on the defined domain, resource, and action.
///// </remarks>
//public class IdentityActionAttribute : Attribute
//{
//    /// <summary>
//    /// Gets or sets the domain associated with the identity action.
//    /// </summary>
//    public string Domain { get => IdentityAction.Domain; set => IdentityAction.Domain = value; }

//    /// <summary>
//    /// Gets or sets the specific resource under a domain that the identity action targets.
//    /// </summary>
//    public string Resource { get => IdentityAction.Resource; set => IdentityAction.Resource = value; }

//    /// <summary>
//    /// Gets or sets the name of the identity action.
//    /// </summary>
//    public string Action { get => IdentityAction.Name; set => IdentityAction.Name = value; }

//    /// <summary>
//    /// Holds the internal representation of the identity action.
//    /// </summary>
//    private IdentityAction IdentityAction { get; }

//    /// <summary>
//    /// Initializes a new instance of the <see cref="IdentityActionAttribute"/> class based on individual domain, resource, and action strings.
//    /// </summary>
//    /// <param name="domain">The domain for the identity action.</param>
//    /// <param name="resource">The resource targeted by the identity action.</param>
//    /// <param name="action">The name of the identity action.</param>
//    public IdentityActionAttribute(string domain, string resource, string action)
//    {
//        IdentityAction = new(domain, resource, action);
//    }

//    /// <summary>
//    /// Initializes a new instance of the <see cref="IdentityActionAttribute"/> class based on an action string.
//    /// </summary>
//    /// <param name="actionString">The action string in the format "domain:resource:action".</param>
//    public IdentityActionAttribute(string actionString, string? description = null)
//    {
//        IdentityAction = new(actionString);
//    }

//    /// <summary>
//    /// Returns a string representation of the identity action in the format "domain:resource:action".
//    /// </summary>
//    /// <returns>A string representation of the identity action.</returns>
//    public override string ToString()
//    {
//        return IdentityAction.ToString();
//    }

//    /// <summary>
//    /// Retrieves the internal <see cref="IdentityAction"/> object.
//    /// </summary>
//    /// <returns>The <see cref="IdentityAction"/> representation of the attribute.</returns>
//    public IdentityAction GetIdentityAction()
//    {
//        return IdentityAction;
//    }
//}
