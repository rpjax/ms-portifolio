using System.Text.Json.Serialization;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="LegalDocument"/> class.
    /// This parameterless constructor is mainly used for serialization purposes.
    /// </summary>
    [JsonConstructor]
    public LegalDocument()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegalDocument"/> class with specified details.
    /// </summary>
    /// <param name="value">The value of the legal document. This parameter is mandatory.</param>
    /// <param name="type">The type of the legal document. It can be null if the document type is not specified.</param>
    /// <param name="issuedAt">The date and time when the legal document was issued. Can be null if not applicable or unknown.</param>
    /// <param name="expiresAt">The date and time when the legal document expires. Can be null if the document does not have an expiration date or if the date is unknown.</param>
    public LegalDocument(
        string value,
        string? type = null,
        DateTime? issuedAt = null,
        DateTime? expiresAt = null)
    {
        Value = value;
        Type = type;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

}
