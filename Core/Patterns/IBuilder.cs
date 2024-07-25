namespace ModularSystem.Core;

/// <summary>
/// Defines a builder interface responsible for creating instances of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBuilder<out T>
{
    /// <summary>
    /// Builds and returns an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T Build();
}
