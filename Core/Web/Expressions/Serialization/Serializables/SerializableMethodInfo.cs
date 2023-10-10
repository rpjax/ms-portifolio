namespace ModularSystem.Web.Expressions;

public class SerializableMethodInfo
{
    public bool IsGeneric { get; set; }
    public string? Name { get; set; }
    public SerializableType? DeclaringType { get; set; }
    public SerializableType? ReturnType { get; set; }
    public List<SerializableType> GenericArguments { get; set; } = new();

    /// <summary>
    /// Type.Fullname + Name
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetFullName()
    {
        if (DeclaringType == null)
        {
            throw new InvalidOperationException();
        }
        if (Name == null)
        {
            throw new InvalidOperationException();
        }

        return $"{DeclaringType.GetFullName()}.{Name}";
    }
}
