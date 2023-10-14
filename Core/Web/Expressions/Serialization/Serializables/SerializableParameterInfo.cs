namespace ModularSystem.Web.Expressions;

public class SerializableParameterInfo
{
    public SerializableType? MethodDeclaringType { get; set; } = null;
    public SerializableMethodInfo? MethodInfo { get; set; }

    public string? ParameterName { get; set; } = null;
    public SerializableType? ParameterType { get; set; } = null;
}
