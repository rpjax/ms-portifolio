namespace ModularSystem.Core;

/// <summary>
/// Defines the possible environment types in which an application can run.
/// </summary>
public enum EnvironmentType
{
    /// <summary>
    /// Represents a development environment. Typically used for testing and local development.
    /// </summary>
    Development,

    /// <summary>
    /// Represents a staging environment. This is often a pre-production environment used for final testing before deploying to production.
    /// </summary>
    Staging,

    /// <summary>
    /// Represents a production environment where the application runs for the end-users.
    /// </summary>
    Production,
}

/// <summary>
/// Indicates the direction of sorting or ordering operations.
/// </summary>
public enum OrderingDirection
{
    /// <summary>
    /// Represents an ascending order, typically from smallest to largest or A to Z.
    /// </summary>
    Ascending,

    /// <summary>
    /// Represents a descending order, typically from largest to smallest or Z to A.
    /// </summary>
    Descending
}

/// <summary>
/// Defines priority levels with associated integer values.
/// </summary>
public enum PriorityLevel : int
{
    /// <summary>
    /// Represents the lowest priority level.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Represents a priority level that's below the standard or normal level.
    /// </summary>
    BelowNormal = 2,

    /// <summary>
    /// Represents the standard or default priority level.
    /// </summary>
    Normal = 3,

    /// <summary>
    /// Represents a priority level that's above the standard or normal level.
    /// </summary>
    AboveNormal = 4,

    /// <summary>
    /// Represents the highest priority level.
    /// </summary>
    High = 5
}
