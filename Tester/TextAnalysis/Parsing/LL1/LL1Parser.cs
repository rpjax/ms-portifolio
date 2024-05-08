using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Parsing.LL1.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing;

public class LL1Context
{
    private LL1Grammar Grammar { get; }
    public LL1InputStream InputStream { get; }
    public LL1Stack Stack { get; }
    public LL1ParsingTable ParsingTable { get; }

    private bool IsInit { get; set; }

    public LL1Context(LL1Grammar grammar, LL1InputStream inputStream, LL1Stack stack)
    {
        Grammar = grammar;
        InputStream = inputStream;
        Stack = stack;
        ParsingTable = grammar.GetParsingTable();
    }

    public void Init()
    {
        if (IsInit)
        {
            throw new InvalidOperationException();
        }

        IsInit = true;
        Stack.Push(Eoi.Instance);
        Stack.Push(Grammar.Start);
    }
}

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
        using var inputStream = new LL1InputStream(input);
        var stack = new LL1Stack();
        var context = new LL1Context(Grammar, inputStream, stack);

        context.Init();

        while (true)
        {
            if (stack.Top is Eoi)
            {
                OnEoi(context);
                break;
            }

            if (stack.Top is Epsilon)
            {
                OnEpsilon(context);
                continue;
            }

            if (stack.Top is Terminal terminal)
            {
                Match(context);
                continue;
            }

            Expand(context);
        }

        Console.WriteLine();
    }

    private void OnEoi(LL1Context context)
    {
        if (context.InputStream.Lookahead is not null)
        {
            throw new Exception($"Unexpected token ({context.InputStream.Lookahead}). Expected EOI.");
        }
    }

    private void OnEpsilon(LL1Context context)
    {
        context.Stack.Pop();
    }

    private void Match(LL1Context context)
    {
        var input = context.InputStream;
        var stack = context.Stack;
        var terminal = context.InputStream.Lookahead!;

        if (input.Lookahead is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }
        if (terminal.TokenType != input.Lookahead.TokenType)
        {
            throw new Exception($"Unexpected token ({input.Lookahead}).");
        }
        if (terminal.Value is not null && terminal.Value != input.Lookahead.Value)
        {
            throw new Exception($"Unexpected token ({input.Lookahead}).");
        }

        input.Consume();
        stack.Pop();
    }

    private void Expand(LL1Context context)
    {
        var input = context.InputStream;
        var stack = context.Stack;
        var parsingTable = context.ParsingTable;

        if (stack.Top is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException("Invalid token at the top of the stack.");
        }
        if (input.Lookahead is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }

        var production = parsingTable.Lookup(nonTerminal, input.Lookahead);

        if (production is null)
        {
            throw new Exception("Syntax error");
        }

        stack.Pop();
        stack.Push(production);
    }
}

/*
    TOMOVE to its own files
*/

public class CstNode
{
    public string Identifier { get; }
    public string[] Attributes { get; }
    public CstNode? Parent { get; }
    public CstNode[] Children { get; }

    public CstNode(string id, string[] attributes, CstNode parent, CstNode[] children)
    {
        Identifier = id;
        Attributes = attributes;
        Parent = parent;
        Children = children;
    }

    public bool IsRoot => Parent is null;
    public bool IsLeaf => Children.Length == 0;
}
