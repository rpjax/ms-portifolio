namespace ModularSystem.Core;

/// <summary>
/// Represents a currency value along with its related currency information. <br/>
/// Implements <see cref="IEquatable{CurrencyValue}"/> for value *only* equality.
/// </summary>
public partial class CurrencyValue : IEquatable<CurrencyValue>
{
    /// <summary>
    /// Gets or sets the value of the currency. <br/>
    /// This value will be rounded to the number of decimal places defined by <see cref="CurrencyInfo.DecimalPlaces"/>.
    /// </summary>
    public decimal Value
    {
        get => _value;
        set => _value = Round(value, Info?.DecimalPlaces);
    }

    /// <summary>
    /// Gets or sets the information related to the currency, such as currency code and symbol.
    /// </summary>
    public CurrencyInfo Info { get; set; }

    private decimal _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyValue"/> class with default settings.
    /// </summary>
    public CurrencyValue()
    {
        Info = CurrencyInfo.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyValue"/> class with the provided parameters. 
    /// </summary>
    /// <remarks>
    /// If no <see cref="CurrencyInfo"/> is provided then <see cref="CurrencyInfo.Default"/> is used instead.
    /// </remarks>
    public CurrencyValue(decimal value, CurrencyInfo? currencyInfo = null)
    {
        Info = currencyInfo ?? CurrencyInfo.Default;
        Value = value;
    }

    /// <summary>
    /// Checks for equality with another <see cref="CurrencyValue"/> object based on its value.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is CurrencyValue currencyValue
            && Equals(currencyValue);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Converts the currency value to a string representation, including its symbol if available.
    /// </summary>
    /// <returns>A string representing the currency value.</returns>
    public override string ToString()
    {
        return $"{Info.Symbol} {Value.ToString("N" + Info.DecimalPlaces)}";
    }

    /// <summary>
    /// Checks for equality with another <see cref="CurrencyValue"/> object based on its value.
    /// </summary>
    /// <param name="other">The other <see cref="CurrencyValue"/> object.</param>
    /// <returns>True if both objects have the same value, otherwise false.</returns>
    public bool Equals(CurrencyValue? other)
    {
        return other?.Value == Value;
    }

    /// <summary>
    /// Rounds a decimal value to the specified number of decimal places, rounding away from zero for midpoint values.
    /// </summary>
    /// <param name="value">The decimal value to be rounded.</param>
    /// <param name="decimalPlaces">The number of decimal places to round to. Defaults to <see cref="CurrencyInfo.DefaultDecimalPlaces"/>.</param>
    /// <returns>The rounded decimal value.</returns>
    public static decimal Round(decimal value, int? decimalPlaces)
    {
        return Math.Round(value, decimalPlaces ?? CurrencyInfo.DefaultDecimalPlaces, MidpointRounding.AwayFromZero);
    }

}

public partial class CurrencyValue
{
    /// <summary>
    /// Subtracts the value of one CurrencyValue object from another.
    /// </summary>
    public static CurrencyValue operator -(CurrencyValue left, CurrencyValue right)
    {
        return new CurrencyValue(left.Value - right.Value, left.Info);
    }

    /// <summary>
    /// Adds the value of two CurrencyValue objects.
    /// </summary>
    public static CurrencyValue operator +(CurrencyValue left, CurrencyValue right)
    {
        return new CurrencyValue(left.Value + right.Value, left.Info);
    }

    /// <summary>
    /// Multiplies the value of two CurrencyValue objects.
    /// </summary>
    public static CurrencyValue operator *(CurrencyValue left, CurrencyValue right)
    {
        return new CurrencyValue(left.Value * right.Value, left.Info);
    }

    /// <summary>
    /// Divides the value of one CurrencyValue object by another.
    /// </summary>
    public static CurrencyValue operator /(CurrencyValue left, CurrencyValue right)
    {
        return new CurrencyValue(left.Value / right.Value, left.Info);
    }

    //*
    // implicit operators
    //*

    /// <summary>
    /// Implicitly converts a decimal value to a CurrencyValue object.
    /// </summary>
    public static implicit operator CurrencyValue(decimal value)
    {
        return new CurrencyValue(value, null);
    }

    /// <summary>
    /// Implicitly converts a CurrencyValue object to a decimal value.
    /// </summary>
    public static implicit operator decimal(CurrencyValue currencyValue)
    {
        return currencyValue.Value;
    }

    //*
    // util static constructos.
    //*

    /// <summary>
    /// Creates a new CurrencyValue object with a specified value in BRL.
    /// </summary>
    public static CurrencyValue Brl(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Brl()
        };
    }

    public static CurrencyValue Usd(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Usd()
        };
    }

    public static CurrencyValue Euro(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Euro()
        };
    }

    public static CurrencyValue Cny(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Cny()
        };
    }

    public static CurrencyValue Gbp(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Gbp()
        };
    }

    public static CurrencyValue Ars(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Ars()
        };
    }

    public static CurrencyValue Jpy(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Jpy()
        };
    }

    public static CurrencyValue Pyg(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Pyg()
        };
    }

    public static CurrencyValue Krw(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Krw()
        };
    }

    public static CurrencyValue Uyu(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Uyu()
        };
    }

    public static CurrencyValue Pen(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Pen()
        };
    }

    public static CurrencyValue Mxn(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Mxn()
        };
    }

    public static CurrencyValue Clp(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Clp()
        };
    }

    public static CurrencyValue Cop(decimal value = 0)
    {
        return new CurrencyValue()
        {
            Value = value,
            Info = CurrencyInfo.Cop()
        };
    }
}

public static class CurrencyValueExtensions
{
    public static CurrencyValue Add(this CurrencyValue self, CurrencyValue value)
    {
        return self + value;
    }

    public static CurrencyValue Subtract(this CurrencyValue self, CurrencyValue value)
    {
        return self - value;
    }
}