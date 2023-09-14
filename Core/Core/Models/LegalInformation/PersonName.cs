namespace ModularSystem.Core;

/// <summary>
/// Represents a person's name, containing a first name and a list of surnames.
/// </summary>
public class PersonName
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the list of surnames for the person.
    /// </summary>
    public List<string> Surnames { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonName"/> class with default values.
    /// </summary>
    public PersonName()
    {
        FirstName = string.Empty;
        Surnames = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonName"/> class with a given first name and surnames.
    /// </summary>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="surnames">An optional list of surnames for the person.</param>
    public PersonName(string firstName, IEnumerable<string>? surnames = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be null or whitespace.", nameof(firstName));
        }

        FirstName = firstName;
        Surnames = surnames?.ToList() ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonName"/> class from a full name.
    /// </summary>
    /// <param name="fullname">The full name of the person, separated by spaces.</param>
    public PersonName(string fullname) : this()
    {
        var split = fullname.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (split.Any())
        {
            FirstName = split[0];
            split.RemoveAt(0);
        }

        foreach (var name in split)
        {
            Surnames.Add(name);
        }
    }

    /// <summary>
    /// Returns the full name of the person.
    /// </summary>
    /// <returns>The full name of the person.</returns>
    public override string ToString()
    {
        return GetFullName();
    }

    /// <summary>
    /// Assembles and returns the full name of the person, including all surnames.
    /// </summary>
    /// <returns>The full name of the person.</returns>
    public string GetFullName()
    {
        return string.Join(" ", new[] { FirstName }.Concat(Surnames));
    }

    /// <summary>
    /// Assembles and returns the initials of the person.
    /// </summary>
    /// <returns>The initials of the person.</returns>
    public string GetInitials()
    {
        var initials = FirstName.Substring(0, 1).ToUpper();

        foreach (var surname in Surnames)
        {
            initials += surname.Substring(0, 1).ToUpper();
        }

        return initials;
    }
}
