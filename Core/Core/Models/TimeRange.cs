namespace ModularSystem.Core;

/// <summary>
/// Represents a range of time with a start and an end point.
/// </summary>
public class TimeRange
{
    /// <summary>
    /// Gets the start date and time of the time range.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the end date and time of the time range.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> class with a specified start and end time.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    /// <exception cref="ArgumentException">Thrown if the end date is earlier than the start date.</exception>
    public TimeRange(DateTime start, DateTime end)
    {
        if (end < start)
        {
            throw new ArgumentException("End date must be later than or equal to the start date.");
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Calculates the duration of the time range.
    /// </summary>
    /// <returns>A <see cref="TimeSpan"/> representing the duration of the time range.</returns>
    public TimeSpan GetDuration()
    {
        return End - Start;
    }

}
