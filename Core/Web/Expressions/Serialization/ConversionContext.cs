using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public class ConversionContext
{
    private ConversionContext? Parent { get; }
    private ReferenceTable ReferenceTable { get; }

    public ConversionContext(ConversionContext? parent = null)
    {
        Parent = parent;
        ReferenceTable = new();
    }

    public ConversionContext CreateChild()
    {
        return new ConversionContext(this);
    }

    public string GetExpressionReferenceId(Expression expression)
    {
        return ReferenceTable.CreateReferenceId(expression);
    }

    public Expression GetExpressionReference(Expression expression)
    {
        var id = GetExpressionReferenceId(expression);

        if (ReferenceTable.HashMap.TryGetValue(id, out var value))
        {
            return value;
        }

        ReferenceTable.HashMap.Add(id, expression);

        return expression;
    }

}

public class ReferenceTable
{
    public Dictionary<string, Expression> HashMap { get; } = new();

    public string CreateReferenceId(Expression expression)
    {
        return $"ref_{expression.GetHashCode()}";
    }

}

public class ReferenceRecord
{
    public string Hash { get; set; }
    public Expression Expression { get; set; }
}