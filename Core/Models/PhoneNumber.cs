namespace ModularSystem.Core;

/// <summary>
/// Represents a phone number with separate components for the country code, area code, and local number.
/// </summary>
public class PhoneNumber
{
    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the area code.
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// Gets or sets the local number.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> class.
    /// </summary>
    public PhoneNumber()
    {
        CountryCode = string.Empty;
        AreaCode = string.Empty;
        Number = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> class.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <param name="areaCode">The area code.</param>
    /// <param name="number">The local number.</param>
    public PhoneNumber(string countryCode, string areaCode, string number)
    {
        CountryCode = countryCode;
        AreaCode = areaCode;
        Number = number;
    }

    /// <summary>
    /// Converts the phone number to its string representation in the E.164 format.
    /// </summary>
    /// <remarks>
    /// The E.164 format consists of a '+' symbol followed by the country code, area code, and local number.
    /// Spaces, dashes, and parentheses are not included.<br/>
    /// For a phone number with CountryCode '1', AreaCode '212', and Number '1234567',
    /// this method will return "+12121234567".
    /// </remarks>
    /// <returns>The phone number formatted as a string in E.164 format.</returns>
    public string ToE164()
    {
        return $"+{CountryCode}{AreaCode}{Number}";
    }

    /// <summary>
    /// Converts the phone number to its string representation in the international E.123 format.
    /// </summary>
    /// <remarks>
    /// The international E.123 format is represented by a '+' symbol, followed by the country code, 
    /// enclosed area code in parentheses, and local number.<br/>
    /// For a phone number with CountryCode '1', AreaCode '212', and Number '1234567',
    /// this method will return "+1 (212) 1234567".
    /// </remarks>
    /// <returns>The phone number formatted as a string in international E.123 format.</returns>
    public string ToE123()
    {
        return $"+{CountryCode} ({AreaCode}) {Number}";
    }

    /// <summary>
    /// Converts the phone number to its string representation in the RFC 3966 format.
    /// </summary>
    /// <remarks>
    /// The RFC 3966 format is used to represent a tel URI (Uniform Resource Identifier).
    /// This method employs the E.164 format as the base and prefixes it with 'tel:'.<br/>
    /// For a phone number with CountryCode '1', AreaCode '212', and Number '1234567',
    /// this method will return "tel:+12121234567".
    /// </remarks>
    /// <returns>The phone number formatted as a string in RFC 3966 format.</returns>
    public string ToRfc3966()
    {
        return $"tel:{ToE164()}";
    }

}
