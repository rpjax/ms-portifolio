namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a base for converting between different representations of data.
/// </summary>
public abstract class Converter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected abstract ParsingContext Context { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ParsingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    /// <param name="additionalData">Additional data related to the exception.</param>
    /// <returns>A new instance of the <see cref="ParsingException"/> class.</returns>
    protected Exception ParsingException(string message, Exception? innerException = null, object? additionalData = null)
    {
        return new ParsingException(message, Context, innerException);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ParsingException"/> class indicating a missing argument.
    /// </summary>
    /// <param name="argumentName">The name of the missing argument.</param>
    /// <returns>A new instance of the <see cref="ParsingException"/> class.</returns>
    protected Exception MissingArgumentException(string argumentName)
    {
        return ParsingException($"The argument '{argumentName}' is required and cannot be null.");
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ParsingException"/> class indicating a constructor was not found.
    /// </summary>
    /// <param name="constructorInfo">The information about the missing constructor.</param>
    /// <returns>A new instance of the <see cref="ParsingException"/> class.</returns>
    protected Exception ConstructorNotFoundException(SerializableConstructorInfo constructorInfo)
    {
        return ParsingException($"Unable to locate a constructor for the type '{constructorInfo.DeclaringType?.FullName}' with the signature: {constructorInfo.GetSignatureString()}.");
    }

    /// <summary>
    /// Creates a custom exception indicating that a specific property could not be found within a type.
    /// </summary>
    /// <param name="propertyInfo">The serializable representation of the property that was not found.</param>
    /// <returns>A <see cref="ParsingException"/> with a detailed error message.</returns>
    protected Exception PropertyNotFoundException(SerializablePropertyInfo propertyInfo)
    {
        return ParsingException($"The property '{propertyInfo.Name}' could not be found in the type '{propertyInfo.DeclaringType?.FullName}'. Ensure that the property exists and is accessible.");
    }

}
