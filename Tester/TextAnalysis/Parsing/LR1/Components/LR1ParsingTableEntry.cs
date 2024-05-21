using ModularSystem.Core.TextAnalysis.Language.Components;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1ParsingTableEntry
{
    public int Id { get; }
    public LR1State State { get; }
    public Dictionary<string, LR1Action> ActionTable { get; }

    public LR1ParsingTableEntry(
        int id,
        LR1State state,
        Dictionary<Symbol, LR1Action> actionsTable)
    {
        Id = id;
        State = state;
        ActionTable = actionsTable
            .ToDictionary(x => CreateKey(x.Key), x => x.Value);
    }

    private static string CreateKey(Symbol symbol)
    {
        return LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.TypeAndValue);
    }

    public override string ToString()
    {
        var actionStrBuilder = new StringBuilder();
        var mainStrBuilder = new StringBuilder();

        foreach (var action in ActionTable)
        {
            actionStrBuilder.AppendLine($"On ({action.Key}) {action.Value.ToString()};");
        }

        mainStrBuilder.AppendLine($"State: {Id}");
        mainStrBuilder.AppendLine(actionStrBuilder.ToString());

        return mainStrBuilder.ToString();
    }

    public LR1Action? Lookup(Symbol symbol)
    {
        var key1 = LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.TypeAndValue);
        var key2 = LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.Type);

        if (ActionTable.TryGetValue(key1, out var action1))
        {
            return action1;
        }

        if (ActionTable.TryGetValue(key2, out var action2))
        {
            return action2;
        }

        return null;
    }

}
