using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core;

/// <summary>
/// Represents the Visitor pattern, which allows a new operation to be added to classes without modifying them. <br/>
/// The Visitor pattern is a way of separating an algorithm from an object structure on which it operates. <br/>
/// A practical result of this separation is the ability to add new operations to existing object structures <br/>
/// without having to modify those structures.<br/>
/// In the context of this interface, the visitor visits an instance of type <typeparamref name="T"/> and
/// potentially performs operations or modifications on it.
/// </summary>
/// <typeparam name="T">The type of the element the visitor can operate on.</typeparam>
public interface IVisitor<T>
{
    /// <summary>
    /// Visits the provided value of type <typeparamref name="T"/> and returns a potentially modified value.
    /// </summary>
    /// <param name="value">The instance of type <typeparamref name="T"/> to visit.</param>
    /// <returns>
    /// The modified instance after the visit, or the original instance if no modifications were made.
    /// If the provided value is null and remains unmodified after the visit, returns null.
    /// </returns>
    [return: NotNullIfNotNull("value")]
    T? Visit(T? value);
}

/// <summary>
/// Represents an asynchronous variation of the Visitor pattern, allowing operations to be added to classes without modifying them. <br/>
/// This interface provides an asynchronous method for visiting an instance of type <typeparamref name="T"/> and potentially <br/>
/// performing asynchronous operations or modifications on it.
/// </summary>
/// <typeparam name="T">The type of the element the asynchronous visitor can operate on.</typeparam>
public interface IAsyncVisitor<T>
{
    /// <summary>
    /// Asynchronously visits the provided value of type <typeparamref name="T"/> and returns a potentially modified value.
    /// </summary>
    /// <param name="value">The instance of type <typeparamref name="T"/> to visit asynchronously.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the modified instance after the visit,
    /// or the original instance if no modifications were made. If the provided value is null and remains unmodified after 
    /// the visit, the task result contains null.
    /// </returns>
    [return: NotNullIfNotNull("value")]
    Task<T?> VisitAsync(T? value);
}