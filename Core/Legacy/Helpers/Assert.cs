namespace ModularSystem.Core.Helpers;

/// <summary>
/// Provides methods to assert conditions, primarily used for validation.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Ensures that a given condition is true.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">Optional. A custom error message to use for the exception when the condition is false.</param>
    /// <exception cref="InvalidOperationException">Thrown when the condition is false.</exception>
    public static void Condition(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message ?? "The specified condition was not met.");
        }
    }

    // Suggestion: Add more specialized assert methods for common scenarios.

    /// <summary>
    /// Ensures that an object is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter or variable. This will be included in the exception if thrown.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public static void NotNull(object? value, string paramName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, $"{paramName} should not be null.");
        }
    }
}
