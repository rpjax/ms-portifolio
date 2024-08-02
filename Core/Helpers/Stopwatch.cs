namespace Aidan.Core.Helpers;

public struct StopwatchMarker
{
    public string Description { get; set; }
    public TimeSpan Timestamp { get; set; }

    public StopwatchMarker(string description, TimeSpan elapsedTime)
    {
        Description = description;
        Timestamp = elapsedTime;
    }
}

public class StopwatchResult<T>
{
    public TimeSpan ElapsedTime { get; }
    public List<StopwatchMarker> Markers { get; } = new List<StopwatchMarker>();
    public T? Data { get; set; }

    public StopwatchResult(TimeSpan elapsedTime, List<StopwatchMarker>? markers = null)
    {
        ElapsedTime = elapsedTime;

        if (markers != null)
        {
            Markers = markers;
        }
    }

    public static implicit operator string(StopwatchResult<T> self)
    {
        return self.ToString();
    }

    public override string ToString()
    {
        var spacer = "    ";
        var str = $"Stopwatch Results:";

        str = $"{str}\n{spacer}Markers:";
        foreach (var marker in Markers)
        {
            double percentage;
            int index = Markers.IndexOf(marker);

            if (index == 0)
            {
                percentage = (marker.Timestamp / ElapsedTime) * 100;
            }
            else
            {
                percentage = (marker.Timestamp - Markers.ElementAt(Markers.IndexOf(marker) - 1).Timestamp) / ElapsedTime * 100;
            }

            str = $"{str}\n{spacer}{spacer}# '{marker.Description}' => elapsed: {marker.Timestamp.TotalSeconds.ToString()} seconds {percentage} %";
        }

        str = $"{str}\n\n{spacer}Total time elapsed: {ElapsedTime.TotalSeconds} seconds.\n";

        return str;
    }
}

public static class Stopwatch
{
    public static StopwatchResult<Void> Run(Action work)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        watch.Start();
        work();
        watch.Stop();

        return new StopwatchResult<Void>(TimeSpan.FromTicks(watch.ElapsedTicks));
    }

    public static StopwatchResult<Void> Run(Action<Action<string>> work)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var markers = new List<StopwatchMarker>();

        watch.Start();

        work((description) =>
        {
            TimeSpan elapsed = TimeSpan.FromTicks(watch.ElapsedTicks);

            markers.Add(new StopwatchMarker(description, elapsed));
        });

        watch.Stop();

        var elaplsed = TimeSpan.FromTicks(watch.ElapsedTicks);
        return new StopwatchResult<Void>(elaplsed, markers);
    }

    public static StopwatchResult<T> Run<T>(Func<Action<string>, T> work)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var markers = new List<StopwatchMarker>();

        watch.Start();

        var data = work((markerDescription) =>
        {
            TimeSpan elapsed = TimeSpan.FromTicks(watch.ElapsedTicks);

            markers.Add(new StopwatchMarker(markerDescription, elapsed));
        });

        watch.Stop();

        var elaplsed = TimeSpan.FromTicks(watch.ElapsedTicks);
        var result = new StopwatchResult<T>(elaplsed, markers);

        result.Data = data;
        return result;
    }
}