namespace ModularSystem.Core;

public static class TimeProvider
{
    public static readonly TimeZoneInfo BrasiliaTimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    public static TimeZoneInfo TimeZone { get; set; } = BrasiliaTimezoneInfo;

    public static DateTime Now()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
    }

    public static DateTime UtcNow()
    {
        return Now().ToUniversalTime();
    }

    public static DateTime BrasiliaTime()
    {
        var timeUtc = DateTime.UtcNow;
        var info = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, info);
    }

    public static DateTime EpochUtc()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static int EpochUtcSeconds()
    {
        return (int)(DateTime.UtcNow - EpochUtc()).TotalSeconds;
    }

}