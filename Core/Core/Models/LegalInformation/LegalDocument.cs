namespace ModularSystem.Core;

/// <summary>
/// Represents a legal document within the system.
/// </summary>
public class LegalDocument
{
    /// <summary>
    /// Gets or sets the type of the legal document. 
    /// The type can be null indicating that the document type is not specified.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the legal document. 
    /// This field is mandatory and initialized as an empty string.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the legal document was issued. <br/>
    /// This property can be null if the issue date is not applicable or unknown.
    /// </summary>
    public DateTime? IssuedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the legal document expires. <br/>
    /// This property can be null, indicating that the document does not have an expiration date or the date is unknown.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
