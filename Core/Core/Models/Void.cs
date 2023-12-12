namespace ModularSystem.Core;

//*
// its an instance of... nothing?
//*

/// <summary>
/// Represents a non-value, providing a concrete type alternative to C#'s void. 
/// This is useful in scenarios like generics where the void type cannot be used directly.
/// </summary>
/// <remarks>
/// The <see cref="Void"/> type effectively encapsulates the concept of "no value" 
/// but still allows the return of a concrete type instance, thus making it useful in various programming scenarios.
/// </remarks>
public sealed class Void
{
    /// <summary>
    /// Provides a convenient static instance of the <see cref="Void"/> class.
    /// </summary>
    public static Void _ => new Void();

    /// <summary>
    /// Initializes a new instance of the <see cref="Void"/> class. 
    /// There's no meaningful state to be held by an instance of this class.
    /// </summary>
    public Void()
    {

    }
}
