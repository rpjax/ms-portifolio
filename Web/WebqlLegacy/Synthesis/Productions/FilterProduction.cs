using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Web.Webql.Synthesis.Productions;

public class AxiomProduction : SymbolProduction
{
    public AxiomProduction() 
    : base("S", new List<SymbolTypeCollection>())
    {
    }
}

public class ObjectProduction : SymbolProduction
{
    public ObjectProduction()
        : base("object", Rule())
    {
    }

    private static List<SymbolTypeCollection> Rule()
    {
        return new List<SymbolTypeCollection>();
    }
}

public class FilterProduction : SymbolProduction
{
    public FilterProduction() 
        : base("filter-expr", Rule())
    {
    }

    private static List<SymbolTypeCollection> Rule()
    {
        return new List<SymbolTypeCollection>();
    }
}

public class ProjectionProduction : SymbolProduction
{
    public ProjectionProduction()
        : base("projection-expr", Rule())
    {
    }

    private static List<SymbolTypeCollection> Rule()
    {
        return new List<SymbolTypeCollection>();
    }
}

public class LambdaProduction : SymbolProduction
{
    public LambdaProduction()
        : base("lambda", Rule())
    {
    }

    private static List<SymbolTypeCollection> Rule()
    {
        return new List<SymbolTypeCollection>();
    }
}

public class ProjectionLambdaProduction : SymbolProduction
{
    public ProjectionLambdaProduction()
        : base("projection-lambda", Rule())
    {
    }

    private static List<SymbolTypeCollection> Rule()
    {
        return new List<SymbolTypeCollection>();
    }
}
