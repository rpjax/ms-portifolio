namespace ModularSystem.Core;

/// <summary>
/// Represents a utility class that provides time-related methods. It serves as a singleton provider for <see cref="DateTime"/>.
/// </summary>
public static class TimeProvider
{
    /// <summary>
    /// The Brasilia time zone information.
    /// </summary>
    public static readonly TimeZoneInfo BrasiliaTimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    /// <summary>
    /// Gets or sets the time zone used by the TimeProvider.
    /// </summary>
    public static TimeZoneInfo TimeZone { get; set; } = BrasiliaTimezoneInfo;

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    /// <returns>The current local date and time.</returns>
    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
    }

    /// <summary>
    /// Gets the current Coordinated Universal Time (UTC).
    /// </summary>
    /// <returns>The current Coordinated Universal Time (UTC).</returns>
    public static DateTime UtcNow()
    {
        return Now().ToUniversalTime();
    }

    /// <summary>
    /// Gets the current Brasilia time.
    /// </summary>
    /// <returns>The current Brasilia time.</returns>
    public static DateTime BrasiliaTime()
    {
        var timeUtc = DateTime.UtcNow;
        var info = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, info);
    }

    /// <summary>
    /// Gets the epoch time in UTC.
    /// </summary>
    /// <returns>The epoch time in UTC.</returns>
    public static DateTime EpochUtc()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Gets the number of seconds since the epoch time in UTC.
    /// </summary>
    /// <returns>The number of seconds since the epoch time in UTC.</returns>
    public static int EpochUtcSeconds()
    {
        return (int)(DateTime.UtcNow - EpochUtc()).TotalSeconds;
    }

}
