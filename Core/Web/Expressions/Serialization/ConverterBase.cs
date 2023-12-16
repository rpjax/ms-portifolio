using ModularSystem.Core.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a base for converting between different representations of data. <br/>
/// This base class includes utility methods for generating exceptions during the conversion process.
/// </summary>
public abstract class ConverterBase
{
    /// <summary>
    /// Creates a new instance of the <see cref="ConversionException"/> class with a specified error message. <br/>
    /// This method is used to encapsulate common exception creation logic.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    /// <param name="additionalData">Additional data related to the exception.</param>
    /// <returns>A new instance of the <see cref="ConversionException"/> class.</returns>
    protected Exception ConversionException(ConversionContext context, string message, Exception? innerException = null, object? additionalData = null)
    {
        return new ConversionException(message, context, innerException, additionalData);
    }

    /// <summary>
    /// Generates an exception indicating that an invalid operation was encountered during the conversion process.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="details">Additional details about the invalid operation.</param>
    /// <returns>An exception with a detailed and contextual error message.</returns>
    protected Exception InvalidOperationException(ConversionContext context, string? details = null)
    {
        string message = $"An invalid operation was encountered during the conversion process. " +
                         $"This may indicate a bug in the conversion logic or a mismatch in the configuration, such as a discrepancy between serialization and deserialization settings. " +
                         $"Details: {details}";
        return ConversionException(context, message);
    }

    /// <summary>
    /// Generates an exception indicating that the provided expression type is not supported.
    /// </summary>
    /// /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="expressionType">The type of the expression that is not supported.</param>
    /// <returns>An exception with a detailed error message.</returns>
    protected Exception ExpressionNotSupportedException(ConversionContext context, ExtendedExpressionType expressionType)
    {
        return ConversionException(context, $"The expression of type '{expressionType}' is not currently supported by the serializer.");
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ConversionException"/> class indicating a missing argument.
    /// </summary>
    /// /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="argumentName">The name of the missing argument.</param>
    /// <returns>A new instance of the <see cref="ConversionException"/> class.</returns>
    protected Exception MissingArgumentException(ConversionContext context, string argumentName)
    {
        return ConversionException(context, $"The argument '{argumentName}' is required and cannot be null.");
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ConversionException"/> class indicating an invalid type name.
    /// </summary>
    /// /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="name">The invalid type name.</param>
    /// <returns>A new instance of the <see cref="ConversionException"/> class.</returns>
    protected Exception InvalidTypeNameException(ConversionContext context, string name)
    {
        return ConversionException(context, $"The type name '{name}' is invalid or not recognized.");
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ConversionException"/> class indicating a constructor was not found.
    /// </summary>
    /// /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="constructorInfo">The information about the missing constructor.</param>
    /// <returns>A new instance of the <see cref="ConversionException"/> class.</returns>
    protected Exception ConstructorNotFoundException(ConversionContext context, SerializableConstructorInfo constructorInfo)
    {
        return ConversionException(context, $"Unable to locate a constructor for the type '{constructorInfo.DeclaringType?.FullName}' with the signature: {constructorInfo.GetSignatureString()}.");
    }

    /// <summary>
    /// Creates a custom exception indicating that a specific property could not be found within a type.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="propertyInfo">The serializable representation of the property that was not found.</param>
    /// <returns>A <see cref="ConversionException"/> with a detailed error message.</returns>
    protected Exception PropertyNotFoundException(ConversionContext context, SerializablePropertyInfo propertyInfo)
    {
        return ConversionException(context, $"The property '{propertyInfo.Name}' could not be found in the type '{propertyInfo.DeclaringType?.FullName}'. Ensure that the property exists and is accessible.");
    }

    /// <summary>
    /// Creates a parsing exception indicating that a specific member was not located within the provided type.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="memberInfo">The serializable representation of the member that was not found.</param>
    /// <returns>A <see cref="ConversionException"/> with a detailed error message.</returns>
    protected Exception MemberNotFoundException(ConversionContext context, SerializableMemberInfo memberInfo)
    {
        return ConversionException(context, $"Failed to locate the member '{memberInfo.Name}' within the type '{memberInfo.DeclaringType?.FullName}'. Please verify the member's existence and its accessibility.");
    }

    /// <summary>
    /// Creates a parsing exception indicating that multiple members with the same name were detected within the provided type, leading to ambiguity.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="memberInfo">The serializable representation of the ambiguous member.</param>
    /// <returns>A <see cref="ConversionException"/> with a detailed error message.</returns>
    protected Exception AmbiguousMemberException(ConversionContext context, SerializableMemberInfo memberInfo)
    {
        return ConversionException(context, $"Detected multiple members named '{memberInfo.Name}' within the type '{memberInfo.DeclaringType?.FullName}'. Please ensure the member's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific method cannot be found.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="methodInfo">The serializable information about the method.</param>
    /// <returns>An exception indicating the method was not found.</returns>
    protected Exception MethodNotFoundException(ConversionContext context, SerializableMethodInfo methodInfo)
    {
        return ConversionException(context, $"Failed to locate the method '{methodInfo.Name}' within the type '{methodInfo.DeclaringType?.FullName}'. Please verify the method's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when a specific method cannot be found.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="declaringType">The type in which the method is expected to be declared.</param>
    /// <param name="methodName">The name of the method that could not be found.</param>
    /// <returns>An exception indicating the method was not found.</returns>
    protected Exception MethodNotFoundException(ConversionContext context, Type declaringType, string methodName)
    {
        return ConversionException(context, $"Failed to locate the method '{methodName}' within the type '{declaringType?.FullName}'. Please verify the method's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when there are multiple methods with the same name, causing ambiguity.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="methodInfo">The serializable information about the method.</param>
    /// <returns>An exception indicating the method is ambiguous.</returns>
    protected Exception AmbiguousMethodException(ConversionContext context, SerializableMethodInfo methodInfo)
    {
        return ConversionException(context, $"Detected multiple methods named '{methodInfo.Name}' within the type '{methodInfo.DeclaringType?.FullName}'. Please ensure the method's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific parameter cannot be found.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="parameterInfo">The serializable information about the parameter.</param>
    /// <returns>An exception indicating the parameter was not found.</returns>
    protected Exception ParameterNotFoundException(ConversionContext context, SerializableParameterInfo parameterInfo)
    {
        return ConversionException(context, $"Failed to locate the parameter '{parameterInfo.ParameterName}' within the type '{parameterInfo.MethodDeclaringType?.FullName}'. Please verify the parameter's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when there are multiple parameters with the same name, causing ambiguity.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="parameterInfo">The serializable information about the parameter.</param>
    /// <returns>An exception indicating the parameter is ambiguous.</returns>
    protected Exception AmbiguousParameterException(ConversionContext context, SerializableParameterInfo parameterInfo)
    {
        return ConversionException(context, $"Detected multiple parameters named '{parameterInfo.ParameterName}' within the type '{parameterInfo.MethodDeclaringType?.FullName}'. Please ensure the parameter's uniqueness to avoid ambiguity.");
    }

    /// <summary>
    /// Generates an exception when a specific type cannot be found within the current running assembly.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="type">The serializable information about the type.</param>
    /// <returns>An exception indicating the type was not found.</returns>
    protected Exception TypeNotFoundException(ConversionContext context, SerializableType type)
    {
        return ConversionException(context, $"Failed to locate the type '{type.GetFullName()}' within the current running assembly. Please verify the type's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when a specific type cannot be found within the current running assembly.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="typeName">The serializable type name.</param>
    /// <returns>An exception indicating the type was not found.</returns>
    protected Exception TypeNotFoundException(ConversionContext context, string typeName)
    {
        return ConversionException(context, $"Failed to locate the type '{typeName}' within the current running assembly. Please verify the type's existence and its accessibility.");
    }

    /// <summary>
    /// Generates an exception when multiple types with the same name are detected within the current running assembly.
    /// </summary>
    /// <param name="context">The conversion context associated with the exception.</param>
    /// <param name="type">The serializable information about the type.</param>
    /// <returns>An exception indicating the ambiguity in type resolution.</returns>
    protected Exception AmbiguousTypeException(ConversionContext context, SerializableType type)
    {
        return ConversionException(context, $"Multiple types named '{type.GetFullName()}' were detected within the current running assembly. Please ensure the type's uniqueness to avoid ambiguity.");
    }

}
