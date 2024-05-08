using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing;

public class LL1Parser
{
    static internal Tokenizer Tokenizer { get; } = new();

    private LL1Grammar Grammar { get; }

    public LL1Parser(LL1Grammar grammar)
    {
        Grammar = grammar;
    }

    public void Parse(string input)
    {
        var stack = new ParsingStack();
        using var stream = new ParsingInputStream(input);
        var parsingTable = Grammar.GetParsingTable();

        stack.Push(Eoi.Instance);
        stack.Push(Grammar.Start);

        while (true)
        {
            if(stack.Top is Terminal terminal)
            {
                if(stream.Current is null)
                {
                    throw new Exception
                }

                // match
                if(terminal != stream.Current)
                {

                }

                continue;
            }

            var nextState = parsingTable.Lookup(stack.Top, stream.Current);
        }
    }
}

public class ParsingInputStream : IDisposable
{
    private IEnumerator<Token?> TokenStream { get; }

    public ParsingInputStream(string input)
    {
        TokenStream = LL1Parser.Tokenizer.Tokenize(input).GetEnumerator();
        TokenStream.MoveNext();
    }

    public Terminal? Current => Peek();

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    public Terminal? Peek()
    {
        if(TokenStream.Current == null)
        {
            return null;
        }

        return new Terminal(TokenStream.Current.Type, TokenStream.Current.Value);
    }

    public void Consume()
    {
        TokenStream.MoveNext();
    }
}

public class ParsingStack
{
    private Stack<Symbol> Symbols { get; } = new();

    public ParsingStack()
    {

    }

    public Symbol? Top => Symbols.Peek();

    public void Push(Symbol symbol)
    {
        Symbols.Push(symbol);
    }

    public Symbol Pop()
    {
        return Symbols.Pop();
    }

    public Symbol? Peek()
    {
        if (Symbols.Count == 0)
        {
            return null;
        }

        return Symbols.Peek();
    }
}

