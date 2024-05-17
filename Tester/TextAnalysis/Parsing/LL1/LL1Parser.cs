using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Parsing.LL1.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing;

public class LL1Parser
{
    private LL1Grammar Grammar { get; }
    private Tokenizer Tokenizer { get; } 

    public LL1Parser(LL1Grammar grammar)
    {
        Grammar = grammar;
        Tokenizer = new Tokenizer();
    }

    public CstNode Parse(string input)
    {
        using var inputStream = new LL1InputStream(input, Tokenizer);
        var stack = new LL1Stack();
        var context = new LL1Context(Grammar, inputStream, stack);

        context.Init();

        while (true)
        {
            if (stack.Top is Eoi)
            {
                return MatchEoi(context);
            }

            if (stack.Top is Epsilon)
            {
                MatchEpsilon(context);
                continue;
            }

            if (stack.Top is Terminal)
            {
                MatchTerminal(context);
                continue;
            }

            if (stack.Top is LL1SemanticSymbol semanticSymbol)
            {
                MatchSemanticSymbol(context, semanticSymbol);
                continue;
            }

            Expand(context);
        }
    }

    private CstNode MatchEoi(LL1Context context)
    {
        if(context.InputStream.Lookahead is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }
        if (context.InputStream.Lookahead.TokenType != TokenType.Eoi)
        {
            throw new Exception($"Unexpected token ({context.InputStream.Lookahead}). Expected EOI.");
        }

        context.Stack.Pop();
        context.InputStream.Consume();
        return context.SyntaxContext.BuildConcreteSyntaxTree();
    }

    private void MatchEpsilon(LL1Context context)
    {
        context.Stack.Pop();
    }

    private void MatchTerminal(LL1Context context)
    {
        var input = context.InputStream;
        var stack = context.Stack;
        var stackTop = (Terminal)context.Stack.Top!;
        var lookahead = context.InputStream.Lookahead;

        if (lookahead is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }
        if (stackTop.TokenType != lookahead.TokenType)
        {
            throw new Exception($"Unexpected token ({input.Lookahead}). Expected {stackTop}.");
        }
        if (stackTop.Value is not null && stackTop.Value != lookahead.Value)
        {
            throw new Exception($"Unexpected token ({input.Lookahead}). Expected {stackTop}.");
        }

        input.Consume();
        stack.Pop();
        context.SyntaxContext.AddAttribute(lookahead);
    }

    private void MatchSemanticSymbol(LL1Context context, LL1SemanticSymbol symbol)
    {
        context.Stack.Pop();

        switch (symbol.Action)
        {
            case LL1SemanticActionType.FinilizeBranch:
                context.SyntaxContext.FinilizeBranch();
                break;

            default:
                throw new InvalidOperationException("Invalid semantic action type.");
        }
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
        stack.Push(LL1SemanticSymbol.FinilizeBranch);
        stack.Push(production);
        context.SyntaxContext.CreateBranch(nonTerminal);
    }

}
