using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

public enum SerializationError
{
    InvalidNodeState,
    InvalidType
}

public class ExpressionSerializerException : Exception
{
    public ExpressionSerializerException(string? message, Exception? inner) : base(message, inner) { }

    public AppException ToAppException(ExceptionCode exceptionCode = ExceptionCode.Internal)
    {
        return new AppException(Message, exceptionCode, this);
    }
}