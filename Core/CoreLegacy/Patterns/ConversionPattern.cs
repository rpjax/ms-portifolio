namespace ModularSystem.Core;

public delegate Out ConversionDelegate<In, Out>(In input);

/// <summary>
/// Represents a contract for converting an instance to a specific type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The target type to which the implementing class instance will be converted.</typeparam>
public interface IConversion<T>
{
    /// <summary>
    /// Converts the current instance of the implementing class to an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The converted instance of type <typeparamref name="T"/>.</returns>
    T Convert();
}

/// <summary>
/// Defines a conversion contract from type <typeparamref name="TInput"/> to type <typeparamref name="TOutput"/>.
/// </summary>
/// <typeparam name="TInput">The source type to be converted.</typeparam>
/// <typeparam name="TOutput">The target type to which the source type will be converted.</typeparam>
public interface IConversion<TInput, TOutput>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="TInput"/> to an instance of type <typeparamref name="TOutput"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TInput"/> to convert.</param>
    /// <returns>The converted instance of type <typeparamref name="TOutput"/>.</returns>
    TOutput Convert(TInput instance);
}


public interface IConverter<T1, T2>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="T1"/> into an instance of type <typeparamref name="T2"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="T1"/> that needs to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T2"/> that represents the converted value of the provided input.</returns>
    T2 Convert(T1 instance);
}

/// <summary>
/// Defines a bidirectional conversion contract between two types, <typeparamref name="T1"/> and <typeparamref name="T2"/>.<br/>
/// This interface allows for the conversion of instances of <typeparamref name="T1"/> to <typeparamref name="T2"/>, and vice-versa.
/// </summary>
/// <typeparam name="T1">The first type involved in the conversion.</typeparam>
/// <typeparam name="T2">The second type involved in the conversion.</typeparam>
public interface IBidirectionalConverter<T1, T2>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="T1"/> into an instance of type <typeparamref name="T2"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="T1"/> that needs to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T2"/> that represents the converted value of the provided input.</returns>
    T2 Convert(T1 instance);

    /// <summary>
    /// Converts the provided input of type <typeparamref name="T2"/> into an instance of type <typeparamref name="T1"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="T2"/> that needs to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T1"/> that represents the converted value of the provided input.</returns>
    T1 Convert(T2 instance);
}

/// <summary>
/// Defines a conversion contract from type <typeparamref name="T1"/> to type <typeparamref name="T2"/>,
/// utilizing an additional state of type <typeparamref name="TState"/>.
/// </summary>
/// <typeparam name="T1">The source type to be converted.</typeparam>
/// <typeparam name="T2">The target type to which the source type will be converted.</typeparam>
/// <typeparam name="TState">The type of the state object used in the conversion process.</typeparam>
public interface IConverter<T1, T2, TState>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="T1"/> using the given state into an instance of type <typeparamref name="T2"/>.
    /// </summary>
    /// <param name="state">The state to be used in the conversion process.</param>
    /// <param name="instance">The instance of <typeparamref name="T1"/> to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T2"/> that represents the converted value of the provided input.</returns>
    T2 Convert(TState state, T1 instance);
}

/// <summary>
/// Defines a bidirectional conversion contract between two types, <typeparamref name="T1"/> and <typeparamref name="T2"/>, <br/>
/// with an additional state of type <typeparamref name="TState"/>. This interface allows conversion in both directions
/// utilizing the given state.
/// </summary>
/// <typeparam name="T1">The first type involved in the conversion.</typeparam>
/// <typeparam name="T2">The second type involved in the conversion.</typeparam>
/// <typeparam name="TState">The type of the state object used in the conversion process.</typeparam>
public interface IBidirectionalConverter<T1, T2, TState>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="T1"/> using the given state into an instance of type <typeparamref name="T2"/>.
    /// </summary>
    /// <param name="state">The state to be used in the conversion process.</param>
    /// <param name="instance">The instance of <typeparamref name="T1"/> that needs to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T2"/> that represents the converted value of the provided input.</returns>
    T2 Convert(TState state, T1 instance);

    /// <summary>
    /// Converts the provided input of type <typeparamref name="T2"/> using the given state into an instance of type <typeparamref name="T1"/>.
    /// </summary>
    /// <param name="state">The state to be used in the conversion process.</param>
    /// <param name="instance">The instance of <typeparamref name="T2"/> that needs to be converted.</param>
    /// <returns>An instance of type <typeparamref name="T1"/> that represents the converted value of the provided input.</returns>
    T1 Convert(TState state, T2 instance);
}

/// <summary>
/// Used to adapt objects between layers.
/// </summary>
/// <typeparam name="TInner"></typeparam>
/// <typeparam name="TOuter"></typeparam>
public interface ILayerAdapter<TInner, TOuter>
{
    /// <summary>
    /// Converts the provided input of type <typeparamref name="TInner"/> to an instance of type <typeparamref name="TOuter"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TInner"/> to convert.</param>
    /// <returns>The converted instance of type <typeparamref name="TOuter"/>.</returns>
    TInner Adapt(TOuter instance);

    /// <summary>
    /// Converts the provided input of type <typeparamref name="TInner"/> to an instance of type <typeparamref name="TOuter"/>.
    /// </summary>
    /// <param name="instance">The instance of <typeparamref name="TInner"/> to convert.</param>
    /// <returns>The converted instance of type <typeparamref name="TOuter"/>.</returns>
    TOuter Present(TInner instance);
}
