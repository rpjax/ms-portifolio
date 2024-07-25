namespace ModularSystem.Web.Expressions;

public class SerializableElementInit
{
    public SerializableMethodInfo? MethodInfo { get; set; }
    public SerializableExpression[] Arguments { get; set; } = Array.Empty<SerializableExpression>();
}
