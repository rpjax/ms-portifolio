using System.Text.Json.Serialization;

namespace ModularSystem.Mailing;

/// <summary>
/// Represents an email address in its constituent parts: username, domain, and extension.
/// </summary>
public class EmailAddress
{
    //*
    // email formatting: username@domain.extension
    //*

    /// <summary>
    /// The username part of the email.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The domain part of the email.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// The extension part of the email (e.g., "com", "org").
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// Default constructor for LINQ providers and JsonSerializer.
    /// </summary>
    [JsonConstructor]
    public EmailAddress()
    {
        //*
        // this constructor exists so that LINQ providers and JsonSerializer can instantiate this object with no params.
        //*

        Username = string.Empty;
        Domain = string.Empty;
        Extension = string.Empty;
    }

    /// <summary>
    /// Constructs an email address from its constituent parts.
    /// </summary>
    /// <param name="username">The username part of the email.</param>
    /// <param name="domain">The domain part of the email.</param>
    /// <param name="extension">The extension part of the email.</param>
    public EmailAddress(string username, string domain, string extension)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain cannot be empty.", nameof(domain));

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension cannot be empty.", nameof(extension));

        Username = username;
        Domain = domain;
        Extension = extension;
    }

    /// <summary>
    /// Constructs an email address from a single string.
    /// </summary>
    /// <param name="email">The email address string.</param>
    public EmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        var parts = email.Split('@');

        if (parts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        Username = parts[0];

        var domainParts = parts[1].Split('.');

        if (domainParts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        Domain = domainParts[0];
        Extension = domainParts[1];
    }

    /// <summary>
    /// Returns an empty Email object.
    /// </summary>
    /// <returns>An empty Email object.</returns>
    public static EmailAddress Empty()
    {
        return new EmailAddress();
    }

    /// <summary>
    /// Returns the email address in its standard format.
    /// </summary>
    /// <returns>The email address as a string in the format "username@domain.extension".</returns>
    public override string ToString()
    {
        return $"{Username}@{Domain}.{Extension}";
    }

    /// <summary>
    /// Returns the email address in a format safe for displaying to end-users.
    /// </summary>
    /// <returns>The email address with the username partially obfuscated, for example "u****e@domain.extension".</returns>
    public string ToSafeDisplayFormat()
    {
        if (string.IsNullOrEmpty(Username) || Username.Length < 2)
        {
            return ToString();
        }

        string maskedUsername = Username[0] + new string('*', Username.Length - 2) + Username[^1];
        return $"{maskedUsername}@{Domain}.{Extension}";
    }

}