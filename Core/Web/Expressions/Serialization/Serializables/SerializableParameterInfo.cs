namespace ModularSystem.Web.Expressions;

public class SerializableParameterInfo
{
    public SerializableType? MethodDeclaringType { get; set; } = null;
    public string? MethodName { get; set; } = null;
    public SerializableType? MethodReturnType { get; set; } = null;
    public SerializableType[] MethodParameters { get; set; } = Array.Empty<SerializableType>();

    public string? ParameterName { get; set; } = null;
    public SerializableType? ParameterType { get; set; } = null;
}
