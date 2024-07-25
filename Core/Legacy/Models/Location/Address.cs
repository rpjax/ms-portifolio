using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents a physical address.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the neighborhood.
    /// </summary>
    public string Neighborhood { get; set; }

    /// <summary>
    /// Gets or sets the street name.
    /// </summary>
    public string StreetName { get; set; }

    /// <summary>
    /// Gets or sets the street number.
    /// </summary>
    public string StreetNumber { get; set; }

    /// <summary>
    /// Gets or sets the ZIP code.
    /// </summary>
    public string ZipCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class with empty fields.
    /// </summary>
    [JsonConstructor]
    public Address(
        string country, 
        string state, 
        string city, 
        string neighborhood, 
        string streetName, 
        string streetNumber, 
        string zipCode) 
    {
        Country = country;
        State = state;
        City = city;
        Neighborhood = neighborhood;
        StreetName = streetName;
        StreetNumber = streetNumber;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class with empty fields.
    /// </summary>
    public Address()
    {
        Country = string.Empty;
        State = string.Empty;
        City = string.Empty;
        Neighborhood = string.Empty;
        StreetName = string.Empty;
        StreetNumber = string.Empty;
        ZipCode = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class from an encoded address string.
    /// </summary>
    /// <param name="encodedAddress">The encoded address.</param>
    public Address(string encodedAddress) : this()
    {
        var entries = encodedAddress.Split(";");

        for (int index = 0; index < entries.Length; index++)
        {
            if (index == 0)
            {
                Country = entries[index].Trim();
            }
            else if (index == 1)
            {
                State = entries[index].Trim();
            }
            else if (index == 2)
            {
                City = entries[index].Trim();
            }
            else if (index == 3)
            {
                Neighborhood = entries[index].Trim();
            }
            else if (index == 4)
            {
                StreetName = entries[index].Trim();
            }
            else if (index == 5)
            {
                StreetNumber = entries[index].Trim();
            }
            else if (index == 6)
            {
                ZipCode = entries[index].Trim();
            }
        }
    }

    /// <summary>
    /// Returns the full address including the country and ZIP code.
    /// </summary>
    /// <returns>The full address in the format: "StreetName, StreetNumber - Neighborhood, City - State, ZipCode - Country".</returns>
    public string ToFullAddress()
    {
        return $"{StreetName}, {StreetNumber} - {Neighborhood}, {City} - {State}, {ZipCode} - {Country}";
    }

    /// <summary>
    /// Returns the full address in national format.
    /// </summary>
    /// <param name="showZipCode">Whether to include the ZIP code in the address.</param>
    /// <returns>The full address in national format, optionally including the ZIP code.</returns>
    public string ToFullAddressNational(bool showZipCode = false)
    {
        return $"{StreetName}, {StreetNumber} - {Neighborhood}, {City} - {State}" + (showZipCode ? $", {ZipCode}" : "");
    }

    /// <summary>
    /// Returns the full address as a semicolon-separated string.
    /// </summary>
    /// <returns>The full address, with components separated by semicolons in the following order: Country;State;City;Neighborhood;StreetName;StreetNumber;ZipCode.</returns>
    public string ToEncoded()
    {
        return $"{Country};{State};{City};{Neighborhood};{StreetName};{StreetNumber};{ZipCode}";
    }

}