using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;

/// <summary>
/// Represents a factory that creates a LR(1) parsing table from a grammar.
/// </summary>
public class LR1ParsingTableFactory : IFactory<Grammar, LR1ParsingTable>
{
    /// <summary>
    /// Creates a LR(1) parsing table from a grammar. 
    /// </summary>
    /// <param name="grammar"></param>
    /// <returns></returns>
    public LR1ParsingTable Create(Grammar grammar)
    {
        var states = LR1Tool.ComputeStatesDictionary(grammar.Productions);
        var entries = new List<LR1ParsingTableEntry>();

        var productions = states.Values
            .SelectMany(s => s.Items)
            .Select(i => i.Production)
            .Distinct()
            ;

        var set = new ProductionSet(
            start: grammar.Start,
            productions: productions.ToArray()
        );
       
        foreach (var entry in states)
        {
            var state = entry.Value;
            var id = entry.Key;

            var actions = ComputeActionsForState(
                set: set, 
                id: id, 
                state: state, 
                computedStates: states
            );

            var newEntry = new LR1ParsingTableEntry(
                id: id, 
                state: state, 
                actionsTable: actions
            );

            entries.Add(newEntry);
        }

        return new LR1ParsingTable(
            entries: entries.ToArray(),
            productions: set.ToArray()
        );
    }

    private Dictionary<Symbol, LR1Action> ComputeActionsForState(
        ProductionSet set,
        int id, 
        LR1State state,
        Dictionary<int, LR1State> computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();

        if (state.IsAcceptingState(set))
        {
            actions.Add(Eoi.Instance, new LR1AcceptAction());
        }

        var symbolItems = state.Items
            .GroupBy(x => x.Symbol)
            .Select(x => x.First())
            .ToArray();

        var stateActions = symbolItems
            .SelectMany(item => ComputeActionsForStateItem(set, id, state, item, computedStates))
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

    private Dictionary<Symbol, LR1Action> ComputeActionsForStateItem(
        ProductionSet set,
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
        if (stackSymbol is null || stackSymbol is Epsilon)
        {
            var productionIndex = set.GetProductionIndex(item.Production);

            if (productionIndex == -1)
            {
                throw new InvalidOperationException("The production does not exist in the production set.");
            }

            foreach (var lookahead in item.Lookaheads)
            {
                if(lookahead == Eoi.Instance && state.IsAcceptingState(set))
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

        var nextStateItem = item.GetNextItem();
        var nextStateSignature = nextStateItem.GetSignature(useLookaheads: true);

        var nextStates = computedStates
            .Where(x => x.Value.Kernel.Any(k => k.ContainsItem(nextStateItem)))
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