﻿using Microsoft.AspNetCore.Http;

namespace Aidan.Web.AccessManagement.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpContext"/> to manage custom identity objects within the HTTP context's items collection. <br/>
/// This facilitates the storage and retrieval of user identity information during the lifecycle of an HTTP request.
/// </summary>
public static class HttpContextIdentityExtensions
{
    /// <summary>
    /// The key used for storing and retrieving the identity object from the <see cref="HttpContext.Items"/> dictionary.
    /// This constant ensures consistent access to the identity object across different components of the application.
    /// </summary>
    public const string HttpContextItemsIdentityKey = "__msweb_identity_object";

    /// <summary>
    /// Sets the user's identity object in the current <see cref="HttpContext"/>, allowing for its retrieval throughout the request's lifecycle.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> associated with the current HTTP request.</param>
    /// <param name="identity">The user's identity object to be stored in the context.</param>
    public static void SetIdentity(this HttpContext context, IIdentity identity)
    {
        context.Items[HttpContextItemsIdentityKey] = identity;
    }

    /// <summary>
    /// Tries to retrieve the user's identity object from the current <see cref="HttpContext"/>. <br/>
    /// This method does not throw an exception if the identity is not found, returning null instead.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> associated with the current HTTP request.</param>
    /// <returns>The user's identity object if found; otherwise, null.</returns>
    public static IIdentity? GetIdentity(this HttpContext context)
    {
        if (context.Items.TryGetValue(HttpContextItemsIdentityKey, out object? value))
        {
            return value as IIdentity;
        }

        return null;
    }

}
