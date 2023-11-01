using Microsoft.AspNetCore.Builder;
using System.Text;
using System.Text.RegularExpressions;

namespace ModularSystem.Core.TextAnalysis;

public enum TextParserAction
{
    Skip,
    Error,
    Shift,
    Reduce,
    Accept
}

public interface ITextParser
{

}

public interface IParserTable
{
    StateTransition? TryGetStateTransition(TextParserState state);
}

public class StateTransition
{
    public string Key { get; }
    public TextParserAction Action { get; }
    public string Goto { get; }
    public ReduceOperation? ReduceOperation { get; }
    public Exception? Exception { get; }

    public StateTransition(string key, TextParserAction action, string @goto, ReduceOperation? reduceOperation)
    {
        Key = key;
        Action = action;
        Goto = @goto;
        ReduceOperation = reduceOperation;
    }
}

public class ReduceOperation
{
    public int Length { get; }
    public Symbol Symbol { get; }
}

public class TextParserState
{
    public string Key { get; }
    public Stack<Symbol> Stack { get; }
    public char[] LookAhead { get; }
}

public class TextParser : IDisposable
{
    private TextSource Source { get; }
    private ContextFreeGrammar Grammar { get; }
    private IParserTable Table { get; }

    private StringBuilder TokenBuilder { get; }

    public TextParser(TextSource source, ContextFreeGrammar grammar)
    {
        Source = source;
        Grammar = grammar;
        TokenBuilder = new StringBuilder(1024);
    }

    public void Dispose()
    {
        Source.Dispose();
    }

    public IEnumerable<Symbol> AsEnumerable()
    {
        yield break;
    }

    public Symbol? GetNextSymbol()
    {
        //Source.Advance();
        //TokenBuilder.Clear();

        // <expr> ::= <term> | <expr><operator><term>
        // <term> ::= <number>
        // <number> ::= <integer> | <float>
        // <float> ::= <integer>"."<integer>
        // <integer> ::= <digit> | <integer><digit>
        // <digit> ::= "0" | "1" | "2" | "3" ...
        // <operator> ::= "+" | "-" | "*" | "/"

        // example: 
        // stack = []; input = "5", "0", "+", "3", ";" (lookahead = "5")
        // stack = [<"5">] => (reduce, becase there is no production for "50")
        // stack = [<digit>] => (reduce, becase there is no production for "<digit>0")
        // stack = [<integer>] => (shift, because there is a production for "<integer><"0">")
        // stack = [<integer><"0">] => (reduce, as determined by the parsing table for the given state)
        // stack = [<integer>] => (reduce, because there is a production for "<integer><"+">")
        // stack = [<number>] (reduce, because there is no production for "<number><"+">")
        // stack = [<term>] (shift, because "term" can't be reduced any further)
        // current state + input => state transition

        //*
        // NOTES:
        // The machine is a state machine responsible for proccesing language symbols.
        // Symbol productions are defined as: The deterministic state transition of the machine. The current state and input are used ti determine the next state.
        // 
        // The lexer: is defined as a BOTTOM-TOP LR(2) parser, it is responsible by the grouping of related charaters, desbribed by recursiveness of the prodcutions, such as: <integer> ::= <digit> | <integer><digit>. It achives this by going from the terminals upwards in the syntax tree, checking against the lookahead for sequences of chars that form non-terminals, like numbers, strings, etc...
        // The reason to use LR(k) with k = 2
        // The syntax parser: is defined by a TOP-BOTTOM responsible by forming the syntax-tree. It does this by comparing the stack-top symbol against the input lookahead symbols. If they dont match the parsing table is looked up to rewrite the current stack-top symbol, using the grammar productions.
        // The parsing table: encapsulates the responsability and details of determining what productions to use.
        //*

        //todo...
        while (true)
        {
            if (true)
            {
                break;
            }


        }

        return null;
    }

}

public static class DefinedSymbols
{
    public const string CompilerGenerated = "compiler-generated";
    //public const string Epsilon = "epsilon";
    //public const string String = "string";
    //public const string AsciiChar = "ascii-char";
}

/// <summary>
/// Reads my custom ".gdef" file. The GDEF extends the BNF format.
/// </summary>
public class GDefReader
{
    public const string FileExtension = "gdef";

    public Dictionary<string, string> GetProdutions(FileInfo gDefFile)
    {
        if (!gDefFile.Exists)
        {
            throw new FileNotFoundException($"The '.{FileExtension}' file was not found at {gDefFile.FullName}");
        }

        var text = File.ReadAllText(gDefFile.FullName);

        var breakLineSplit = text.Split("\r");

        breakLineSplit = breakLineSplit
            .RemoveWhere(x => x == "\r" || x == "\n")
            .ToArray();

        var productions = new Dictionary<string, string>(breakLineSplit.Length / 2);

        foreach (var item in breakLineSplit)
        {
            var split = item.Split("::=");

            if(split.Length != 2)
            {
                throw new InvalidOperationException();
            }

            var lhs = split[0].Replace("\r", "").Replace("\n", "");
            var rhs = split[1].Replace("\r", "").Replace("\n", "");

            productions.Add(lhs, rhs);
        }

        return productions;
    }

    public IEnumerable<KeyValuePair<string, string>> GetTokens(Dictionary<string, string> productions)
    {
        var regex = new Regex(@"^\[[a-zA-Z-]+\]\s*$");
        return productions
            .Where(x => regex.IsMatch(x.Key));
    }

    public List<ProductionDefinition> GetDefinitions(FileInfo gDefFile)
    {
        var tokens = GetTokens(GetProdutions(gDefFile));
        var sentenses = new List<ProductionSentense>();
        var defs = new List<ProductionDefinition>();

        foreach (var item in tokens)
        {
            var sentensesStrings = item.Value.Split("|");
            var definition = new ProductionDefinition(item.Key);

            foreach (var sentenseStr in sentensesStrings)
            {
                var sentense = new ProductionSentense();
                var regex = new Regex(@"(\{?\[[^\]]+\]\}?)|('[^']*')|(""[^""]*"")|(\{[^}]+\})|\$");
                var matches = regex.Matches(sentenseStr);

                foreach (Match match in matches)
                {
                    sentense.Words.Add(new(match.Value));
                }

                definition.Sentenses.Add(sentense);
            }

            defs.Add(definition);
        }

        return defs;
    }
}

public class ProductionDefinition
{
    public string Name { get; }
    public List<ProductionSentense> Sentenses { get; } = new();

    public ProductionDefinition(string name)
    {
        Name = name;
    }
}

public class ProductionSentense
{
    public List<ProductionWord> Words { get; } = new();
}

public class ProductionWord
{
    public string Value { get; }

    public ProductionWord(string value)
    {
        Value = value;
    }
}