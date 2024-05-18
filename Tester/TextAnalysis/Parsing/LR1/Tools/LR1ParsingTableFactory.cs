using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;

public class LR1ParsingTableFactory : IFactory<LR1ParsingTable>
{
    private ProductionSet Set { get; }

    public LR1ParsingTableFactory(ProductionSet set)
    {
        Set = set;
    }

    public LR1ParsingTable Create()
    {
        var states = LR1Tool.ComputeStateDictionary(Set);
        var entries = new List<LR1ParsingTableEntry>();

        foreach (var entry in states)
        {
            var state = entry.Value;
            var id = entry.Key;
            var actions = CreateActionsForState(id, state, states);
            var newEntry = new LR1ParsingTableEntry(id, actions);

            entries.Add(newEntry);
        }

        return new LR1ParsingTable(
            entries: entries.ToArray(),
            productions: Set.Productions.ToArray()
        );
    }

    private Dictionary<Symbol, LR1Action> CreateActionsForState(
        int id, 
        LR1State state,
        Dictionary<int, LR1State> computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();

        if (state.IsAcceptingState(Set))
        {
            actions.Add(Eoi.Instance, new LR1AcceptAction());
        }

        var symbolItems = state.Items
            .GroupBy(x => x.Symbol)
            .Select(x => x.First())
            .ToArray();

        var stateActions = symbolItems
            .SelectMany(item => CreateActionsForStateItem(id, state, item, computedStates))
            .ToArray();

        foreach (var action in stateActions)
        {
            if (actions.ContainsKey(action.Key))
            {
                throw new Exception($"Conflict at state {id} with symbol {action.Key}.");
            }

            actions.Add(action.Key, action.Value);
        }

        return actions;
    }

    private Dictionary<Symbol, LR1Action> CreateActionsForStateItem(
        int id,
        LR1State state,
        LR1Item item,
        Dictionary<int, LR1State> computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();
        var stackSymbol = item.Symbol;

        /*
         * Create reduce actions for all lookaheads of the item.
         */
        if (stackSymbol is null)
        {
            var productionIndex = Set.GetProductionIndex(item.Production);

            if (productionIndex == -1)
            {
                throw new InvalidOperationException("The production does not exist in the production set.");
            }

            foreach (var lookahead in item.Lookaheads)
            {
                if(lookahead == Eoi.Instance && state.IsAcceptingState(Set))
                {
                    continue;
                }

                actions.Add(lookahead, new LR1ReduceAction(productionIndex));
            }
            
            return actions;
        }

        /*
         * Create shift or goto actions for the stack symbol.
         */

        var nextStateSignature = item.GetNextItem().GetSignature(useLookaheads: true);

        var nextStates = computedStates
            .Where(x => x.Value.Kernel[0].GetSignature(useLookaheads: true) == nextStateSignature)
            .ToArray();
            ;

        if(nextStates.Length == 0)
        {
            throw new InvalidOperationException("The next state does not exist.");
        }
        if(nextStates.Length > 1)
        {
            throw new InvalidOperationException("The next state is ambiguous.");
        }

        var nextStateId = nextStates[0].Key;

        if (nextStateId == -1)
        {
            throw new InvalidOperationException("The next state does not exist.");
        }

        if (stackSymbol.IsTerminal)
        {
            if (actions.ContainsKey(stackSymbol))
            {
                throw new Exception($"Conflict at state {id} with symbol {stackSymbol}.");
            }

            actions.Add(stackSymbol, new LR1ShiftAction(nextStateId));
        }
        else
        {
            if (actions.ContainsKey(stackSymbol))
            {
                throw new Exception($"Conflict at state {id} with symbol {stackSymbol}.");
            }

            actions.Add(stackSymbol, new LR1GotoAction(nextStateId));
        }

        return actions;
    }
}
