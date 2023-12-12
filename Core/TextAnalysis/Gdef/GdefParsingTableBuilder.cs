namespace ModularSystem.Core.TextAnalysis.Gdef;

public class GdefParsingTableBuilder
{
    private GdefFile GdefFile { get; }

    public GdefParsingTableBuilder(GdefFile gdefFile)
    {
        GdefFile = gdefFile;
    }

    public void Build()
    {
        var lr0States = new List<StateTransition>();
        var productions = GdefFile.Productions;

        foreach (var production in productions)
        {
            if (
                production.Name != "S"
                && production.Patterns.Any(p => p.Fragments.Any(f => f is EpsilonFragment)))
            {

            }

            foreach (var pattern in production.Patterns)
            {

            }

        }
    }

    IEnumerable<ProductionDefinition> HandleEpsilonProductions(IEnumerable<ProductionDefinition> productions)
    {
        //*
        // steps: 
        // order productions by their hierarchy in the syntax tree, terminals up.
        //

        return productions;
    }
}
