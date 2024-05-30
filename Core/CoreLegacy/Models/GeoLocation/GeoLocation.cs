namespace ModularSystem.Core;

/// <summary>
/// Represents the geographic location of an object in terms of latitude, longitude, altitude, speed, and course.
/// </summary>
public class GeoLocation
{
    /// <summary>
    /// Gets or sets the latitude of the location in degrees. 
    /// Valid values range from -90 to 90.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the location in degrees.
    /// Valid values range from -180 to 180.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the altitude of the location in meters above sea level.
    /// </summary>
    public double Altitude { get; set; }

    /// <summary>
    /// Gets or sets the speed of the object at this location in meters per second (m/s).
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// Gets or sets the course of the object at this location, in degrees from 0 to 360.
    /// </summary>
    public double Course { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoLocation"/> class with default values.
    /// </summary>
    public GeoLocation()
    {
        Latitude = 0;
        Longitude = 0;
        Altitude = 0;
        Speed = 0;
        Course = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoLocation"/> class with specified values.
    /// </summary>
    /// <param name="latitude">The latitude in degrees.</param>
    /// <param name="longitude">The longitude in degrees.</param>
    /// <param name="altitude">The altitude in meters.</param>
    /// <param name="speed">The speed in meters per second.</param>
    /// <param name="course">The course in degrees.</param>
    public GeoLocation(double latitude, double longitude, double altitude = 0, double speed = 0, double course = 0)
    {
        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentException("Invalid latitude value. It should be between -90 and 90 degrees.");
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentException("Invalid longitude value. It should be between -180 and 180 degrees.");
        }

        if (course < 0 || course > 360)
        {
            throw new ArgumentException("Invalid course value. It should be between 0 and 360 degrees.");
        }

        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        Speed = speed;
        Course = course;
    }
}
