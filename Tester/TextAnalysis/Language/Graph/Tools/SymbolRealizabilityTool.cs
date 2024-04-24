using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public class SymbolRealizabilityTool
{
    public NonTerminal[] Execute(ProductionSet set)
    {
        var realizableSymbols = new List<Symbol>();
        var progress = false;
        var workingSet = set.Copy();

        while (true)
        {
            foreach (var production in workingSet.Copy())
            {
                if(realizableSymbols.Contains(production.Head))
                {
                    workingSet.RemoveProductions(production);
                    continue;
                }

                var isRealizable = production.Body
                    .All(x => x.IsTerminal || realizableSymbols.Contains(x));

                if(isRealizable)
                {
                    realizableSymbols.Add(production.Head);
                    progress = true;
                    workingSet.RemoveProductions(production);
                }
            }

            if(!progress)
            {
                break;
            }

            progress = false;
        }

        var unrealizableSymbols = set
            .Select(x => x.Head)
            .Distinct()
            .Except(realizableSymbols)
            .Select(x => x.AsNonTerminal())
            .ToArray();

        return unrealizableSymbols;
    }

    //private Sentence[] GetTerminalSentences(ProductionSet set, NonTerminal nonTerminal)
    //{
    //    var sentences = new List<Sentence>();
    //    var expansions = set.Lookup(nonTerminal)
    //        .ToArray();

    //    foreach (var production in expansions)
    //    {
    //        foreach(var symbol in production.Body)
    //        {
    //            var tree = GraphBuilder.CreateDerivationTree(set);

    //            var leafSentences = tree.GetLeafs()
    //                .GroupBy(x => x.Production)
    //                .DistinctBy(x => x.Key)
    //                .Select(x => new Sentence(x.ToArray()))
    //        }
    //    }
    //}

    private void DeriveRecursevly(
        ProductionSet set, 
        Sentence sentence, 
        Stack<Derivation> stack)
    {
        
    }

}
