namespace ModularSystem.Webql.Components;

public abstract class TableBase<TEntry> where TEntry : class
{
    private Dictionary<string, TEntry> Table { get; } = new();

    public TEntry? TryGetEntry(string identifier)
    {
        if (Table.TryGetValue(identifier, out var value))
        {
            return value;
        }

        return null;
    }

    public void AddEntry(string identifier, TEntry value)
    {
        Table.Add(identifier, value);
    }

    public bool ContainsKey(string identifier) 
    { 
        return Table.ContainsKey(identifier);
    }

}
