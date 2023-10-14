using ModularSystem.Core.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a base for converting between different representations of data.
/// </summary>
public abstract class ConverterBase
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected abstract ConversionContext Context { get; }

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
    /// Generates an exception indicating that the provided expression type is not supported.
    /// </summary>
    /// <param name="expressionType">The type of the expression that is not supported.</param>
    /// <returns>An exception with a detailed error message.</returns>
    protected Exception ExpressionNotSupportedException(ExtendedExpressionType expressionType)
    {
        return ParsingException($"The expression of type '{expressionType}' is not currently supported by the serializer.");
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
    /// Creates a new instance of the <see cref="ParsingException"/> class indicating an invalid type name.
    /// </summary>
    /// <param name="name">The invalid type name.</param>
    /// <returns>A new instance of the <see cref="ParsingException"/> class.</returns>
    protected Exception InvalidTypeNameException(string name)
    {
        return ParsingException($"The type name '{name}' is invalid or not recognized.");
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

    /// <summary>
    /// Creates a parsing exception indicating that a specific member was not located within the provided type.
    /// </summary>
    /// <param name="memberInfo">The serializable representation of the member that was not found.</param>
    /// <returns>A <see cref="ParsingException"/> with a detailed error message.</returns>
    protected Exception MemberNotFoundException(SerializableMemberInfo memberInfo)
    {
        return ParsingException($"Failed to locate the member '{memberInfo.Name}' within the type '{memberInfo.DeclaringType?.FullName}'. Please verify the member's existence and its accessibility.");
    }

    /// <summary>
    /// Creates a parsing exception indicating that multiple members with the same name were detected within the provided type, leading to ambiguity.
    /// </summary>
    /// <param name="memberInfo">The serializable representation of the ambiguous member.</param>
    /// <returns>A <see cref="ParsingException"/> with a detailed error message.</returns>
    protected Exception AmbiguousMemberException(SerializableMemberInfo memberInfo)
    {
        return ParsingException($"Detected multiple members named '{memberInfo.Name}' within the type '{memberInfo.DeclaringType?.FullName}'. Please ensure the member's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific method cannot be found.
    /// </summary>
    /// <param name="methodInfo">The serializable information about the method.</param>
    /// <returns>An exception indicating the method was not found.</returns>
    protected Exception MethodNotFoundException(SerializableMethodInfo methodInfo)
    {
        return ParsingException($"Failed to locate the method '{methodInfo.Name}' within the type '{methodInfo.DeclaringType?.FullName}'. Please verify the method's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when a specific method cannot be found.
    /// </summary>
    /// <param name="declaringType">The type in which the method is expected to be declared.</param>
    /// <param name="methodName">The name of the method that could not be found.</param>
    /// <returns>An exception indicating the method was not found.</returns>
    protected Exception MethodNotFoundException(Type declaringType, string methodName)
    {
        return ParsingException($"Failed to locate the method '{methodName}' within the type '{declaringType?.FullName}'. Please verify the method's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when there are multiple methods with the same name, causing ambiguity.
    /// </summary>
    /// <param name="methodInfo">The serializable information about the method.</param>
    /// <returns>An exception indicating the method is ambiguous.</returns>
    protected Exception AmbiguousMethodException(SerializableMethodInfo methodInfo)
    {
        return ParsingException($"Detected multiple methods named '{methodInfo.Name}' within the type '{methodInfo.DeclaringType?.FullName}'. Please ensure the method's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific parameter cannot be found.
    /// </summary>
    /// <param name="parameterInfo">The serializable information about the parameter.</param>
    /// <returns>An exception indicating the parameter was not found.</returns>
    protected Exception ParameterNotFoundException(SerializableParameterInfo parameterInfo)
    {
        return ParsingException($"Failed to locate the parameter '{parameterInfo.ParameterName}' within the type '{parameterInfo.MethodDeclaringType?.FullName}'. Please verify the parameter's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when there are multiple parameters with the same name, causing ambiguity.
    /// </summary>
    /// <param name="parameterInfo">The serializable information about the parameter.</param>
    /// <returns>An exception indicating the parameter is ambiguous.</returns>
    protected Exception AmbiguousParameterException(SerializableParameterInfo parameterInfo)
    {
        return ParsingException($"Detected multiple parameters named '{parameterInfo.ParameterName}' within the type '{parameterInfo.MethodDeclaringType?.FullName}'. Please ensure the parameter's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific type cannot be found within the current running assembly.
    /// </summary>
    /// <param name="type">The serializable information about the type.</param>
    /// <returns>An exception indicating the type was not found.</returns>
    protected Exception TypeNotFoundException(SerializableType type)
    {
        return ParsingException($"Failed to locate the type '{type.GetQualifiedFullName()}' within the current running assembly. Please verify the type's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when a specific type cannot be found within the current running assembly.
    /// </summary>
    /// <param name="typeName">The serializable type name.</param>
    /// <returns>An exception indicating the type was not found.</returns>
    protected Exception TypeNotFoundException(string typeName)
    {
        return ParsingException($"Failed to locate the type '{typeName}' within the current running assembly. Please verify the type's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when multiple types with the same name are detected within the current running assembly.
    /// </summary>
    /// <param name="type">The serializable information about the type.</param>
    /// <returns>An exception indicating the ambiguity in type resolution.</returns>
    protected Exception AmbiguousTypeException(SerializableType type)
    {
        return ParsingException($"Multiple types named '{type.GetFullName()}' were detected within the current running assembly. Please ensure the type's uniqueness to avoid ambiguity.");
    }

}
