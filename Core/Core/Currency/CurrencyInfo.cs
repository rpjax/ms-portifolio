namespace ModularSystem.Core;

/// <summary>
/// Represents currency information such as the currency code and symbol.
/// </summary>
public class CurrencyInfo
{
    /// <summary>
    /// The default number of decimal places used for rounding the currency value.
    /// </summary>
    public const int DefaultDecimalPlaces = 2;

    /// <summary>
    /// Gets or sets the default <see cref="CurrencyInfo"/> object to be used by the <see cref="CurrencyValue"/> class when no specific currency information is provided.
    /// </summary>
    /// <value>
    /// The default <see cref="CurrencyInfo"/> object. Initialized to United States Dollar (USD) by default.
    /// </value>
    /// <remarks>
    /// This property is designed to provide a global setting that specifies the currency information to be used by instances of the <see cref="CurrencyValue"/> class when no currency is explicitly set.
    /// It is initialized to USD by default, but you can change this value to suit the needs of your application.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Using the default CurrencyInfo (USD by default)
    /// var value1 = new CurrencyValue(10);  // Assumes USD if Default is not changed
    ///
    /// // Changing the default CurrencyInfo
    /// CurrencyInfo.Default = CurrencyInfo.Euro();
    ///
    /// // Now using the new default CurrencyInfo (Euro)
    /// var value2 = new CurrencyValue(10);  // Assumes Euro as it's now the default
    /// </code>
    /// </example>
    public static CurrencyInfo Default { get; set; } = Usd();


    /// <summary>
    /// Gets or sets the ISO currency code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the number of decimal places to use for rounding the currency value.
    /// </summary>
    public int DecimalPlaces { get; set; }

    /// <summary>
    /// Default parameterless constructor.
    /// </summary>
    public CurrencyInfo()
    {
        Code = null;
        Symbol = null;
        DecimalPlaces = DefaultDecimalPlaces;
    }

    /// <summary>
    /// Provides information for the Brazilian Real (BRL).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with BRL settings.</returns>
    public static CurrencyInfo Brl()
    {
        return new()
        {
            Code = "BRL",
            Symbol = "R$",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the United States Dollar (USD).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with USD settings.</returns>
    public static CurrencyInfo Usd()
    {
        return new()
        {
            Code = "USD",
            Symbol = "US$",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the Euro (EUR).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with EUR settings.</returns>
    public static CurrencyInfo Euro()
    {
        return new()
        {
            Code = "EUR",
            Symbol = "€",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the British Pound (GBP).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with GBP settings.</returns>
    public static CurrencyInfo Gbp()
    {
        return new()
        {
            Code = "GBP",
            Symbol = "£",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the Japanese Yen (JPY).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with JPY settings.</returns>
    public static CurrencyInfo Jpy()
    {
        return new()
        {
            Code = "JPY",
            Symbol = "¥",
            DecimalPlaces = 0
        };
    }

    /// <summary>
    /// Provides currency information for Chinese Yuan.
    /// </summary>
    /// <returns>A <see cref="CurrencyInfo"/> object for Chinese Yuan.</returns>
    public static CurrencyInfo Cny()
    {
        return new()
        {
            Code = "CNY",
            Symbol = "¥",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides currency information for Paraguayan Guarani.
    /// </summary>
    /// <returns>A <see cref="CurrencyInfo"/> object for Paraguayan Guarani.</returns>
    public static CurrencyInfo Pyg()
    {
        return new()
        {
            Code = "PYG",
            Symbol = "₲",
            DecimalPlaces = 0
        };
    }

    /// <summary>
    /// Provides currency information for Uruguayan Peso.
    /// </summary>
    /// <returns>A <see cref="CurrencyInfo"/> object for Uruguayan Peso.</returns>
    public static CurrencyInfo Uyu()
    {
        return new()
        {
            Code = "UYU",
            Symbol = "$U",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides currency information for South Korean Won.
    /// </summary>
    /// <returns>A <see cref="CurrencyInfo"/> object for South Korean Won.</returns>
    public static CurrencyInfo Krw()
    {
        return new()
        {
            Code = "KRW",
            Symbol = "₩",
            DecimalPlaces = 0
        };
    }

    /// <summary>
    /// Provides information for the Argentine Peso (ARS).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with ARS settings.</returns>
    public static CurrencyInfo Ars()
    {
        return new()
        {
            Code = "ARS",
            Symbol = "$",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the Mexican Peso (MXN).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with MXN settings.</returns>
    public static CurrencyInfo Mxn()
    {
        return new()
        {
            Code = "MXN",
            Symbol = "$",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the Colombian Peso (COP).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with COP settings.</returns>
    public static CurrencyInfo Cop()
    {
        return new()
        {
            Code = "COP",
            Symbol = "$",
            DecimalPlaces = 2
        };
    }

    /// <summary>
    /// Provides information for the Chilean Peso (CLP).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with CLP settings.</returns>
    public static CurrencyInfo Clp()
    {
        return new()
        {
            Code = "CLP",
            Symbol = "$",
            DecimalPlaces = 0
        };
    }

    /// <summary>
    /// Provides information for the Peruvian Sol (PEN).
    /// </summary>
    /// <returns>A CurrencyInfo object initialized with PEN settings.</returns>
    public static CurrencyInfo Pen()
    {
        return new()
        {
            Code = "PEN",
            Symbol = "S/",
            DecimalPlaces = 2
        };
    }
}
