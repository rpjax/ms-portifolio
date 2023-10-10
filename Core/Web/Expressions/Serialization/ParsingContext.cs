using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

public class ParsingContext
{
    public string[] Stack { get; }

    public ParsingContext(string label, string[]? stack = null)
    {
        Stack = stack != null 
            ? new List<string>(stack).FluentAdd(label).ToArray() 
            : new[] { label }; 
    }

    public ParsingContext CreateChild(string label)
    {
        return new ParsingContext(label, Stack);
    }

    public override string ToString()
    {
        return string.Join("->", Stack);
    }
}
