namespace ModularSystem.Web.Expressions;

public class SerializableParameterInfo
{
    public SerializableType? DeclaringType { get; set; }
    public string? MethodName { get; set; }
    public SerializableType[] MethodSignature { get; set; } = Array.Empty<SerializableType>();
    public string? ParameterName { get; set; }
}
