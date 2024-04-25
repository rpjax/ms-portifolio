namespace ModularSystem.Core.TextAnalysis.Language.Components;

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public class GrammarDefinition
{
    private ProductionSet OriginalProductionSet { get; }
    private ProductionSet WorkingProductionSet { get; }
    internal TransformationRecordCollection Transformations { get; } 

    public GrammarDefinition(ProductionRule[] productions, NonTerminal? start = null)
    {
        var set = new ProductionSet(productions);

        if (start is not null)
        {
            set.Start = start;
        }

        OriginalProductionSet = set;
        WorkingProductionSet = set.Copy();
        Transformations = new();
    }

    public ProductionSet Productions => WorkingProductionSet;
    public NonTerminal Start => Productions.Start
        ?? throw new InvalidOperationException("The start symbol is not defined.");

    public override string ToString()
    {
        var productionGroups = Productions
            .GroupBy(x => x.Head.Name);

        var productionsStr = productionGroups
            .Select(x => string.Join(Environment.NewLine, x.Select(y => y.ToString())))
            .ToArray()
            ;

        return string.Join(Environment.NewLine, productionsStr);
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

    public ProductionSet GetOriginalProductionSet()
    {
        return OriginalProductionSet.Copy();
    }

    public TransformationRecordCollection GetTransformationRecords()
    {
        return Transformations.Copy();
    }

}
