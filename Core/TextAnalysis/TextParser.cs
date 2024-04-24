using System.Text;

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
    public LegacySymbol Symbol { get; }
}

public class TextParserState
{
    public string Key { get; }
    public Stack<LegacySymbol> Stack { get; }
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

    public IEnumerable<LegacySymbol> AsEnumerable()
    {
        yield break;
    }

    public LegacySymbol? GetNextSymbol()
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

