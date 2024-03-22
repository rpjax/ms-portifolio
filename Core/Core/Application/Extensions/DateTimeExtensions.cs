namespace ModularSystem.Core;

/// <summary>
/// A static class containing extension methods for DateTime.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime instance to the number of seconds since the Unix epoch (January 1, 1970, 00:00:00 UTC).
    /// </summary>
    /// <param name="dateTime">The DateTime instance to be converted.</param>
    /// <param name="timeZoneInfo">Optional. The time zone information for the DateTime instance. If null, the local time zone is used.</param>
    /// <returns>A long representing the total number of seconds elapsed since the Unix epoch.</returns>
    /// <remarks>
    /// The method first converts the DateTime instance to Coordinated Universal Time (UTC) <br/>
    /// before calculating the duration since the Unix epoch. This ensures that the time zone <br/>
    /// of the original DateTime does not affect the calculation.
    /// </remarks>
    public static long ToEpoch(this DateTime dateTime, TimeZoneInfo? timeZoneInfo = null)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
        {
            if (timeZoneInfo != null)
            {
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);
                dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo ?? TimeZoneInfo.Local);
            }
            else
            {
                dateTime = dateTime.ToUniversalTime();
            }
        }

        // Define the start of the Unix epoch.
        var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Calculate the duration from the epoch start to the dateTime.
        var duration = dateTime - epochStart;

        // Return the total seconds as a long.
        return (long)duration.TotalSeconds;
    }

}
