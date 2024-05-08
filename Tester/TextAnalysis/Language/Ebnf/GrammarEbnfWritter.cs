using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Text.RegularExpressions;

namespace ModularSystem.Core.TextAnalysis.Language.Ebnf;

/*
 *  Grammar representation of a context-free grammar (type 2). 
 *  A context-free grammar can be defined as a 4-tuple G = (N, T, P, S), where:
 *  - N is a set of non-terminal symbols.
 *  - T is a set of terminal symbols.
 *  - P is a set of production rules.
 *  - S is the start symbol.
 */

public class GrammarEbnfWritter
{
    private Grammar Grammar { get; set; }

    public GrammarEbnfWritter(Grammar grammar)
    {
        Grammar = grammar;
    }

    public void Write(TextWriter writer)
    {
        var productionGroups = Grammar.Productions
            .GroupBy(x => x.Head.Name);

        foreach (var group in productionGroups)
        {
            var lhs = group.Key;
            var productionBodies = group
                .Select(x => x.Body)
                .ToArray();

            var firstBody = productionBodies.FirstOrDefault();

            writer.Write($"{lhs} = ");

            for (int i = 0; i < productionBodies.Length; i++)
            {
                if(i > 0)
                {
                    writer.Write(" | ");
                }

                writer.Write(Stringify(productionBodies[i]));
            }

            writer.Write(";\n");
        }
    }

    public string Write()
    {
        using var writer = new StringWriter();
        Write(writer);
        return writer.ToString();
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        Write(writer);
        return writer.ToString();
    }

    private string Stringify(Symbol[]? productionSymbols)
    {
        if(productionSymbols is null)
        {
            return "";
        }

        return string.Join(" ", productionSymbols.Select(x => x.ToString()));
    }
}

public class EbnfParser
{
    // Regex updated to match comments starting with `(*` and ending with `*)`
    private static Regex CommentRegex = new Regex(@"\(\*.*?\*\)", RegexOptions.Singleline);

    private FileInfo FileInfo { get; }
    private Tokenizer Tokenizer { get; }

    public EbnfParser(FileInfo file)
    {
        FileInfo = file;
        Tokenizer = new Tokenizer();
    }

    public Grammar Parse()
    {
        var text = File.ReadAllText(FileInfo.FullName);

        text = CommentRegex.Replace(text, "");
        
        var productionSplit = text.Split(";");

        var productions = productionSplit
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => EbnfProductionParser.Parse(x))
            .ToArray();

        if(productions.Length == 0)
        {
            throw new InvalidOperationException("No productions found in grammar.");
        }

        var start = productions.First().Head;

        return new Grammar(start, productions);
    }
}

public class EbnfProductionParser
{
    public static ProductionRule Parse(string production)
    {
        var parts = production.Split("=");
        var lhs = parts[0].Trim();
        var rhs = parts[1].Trim();

        var body = ParseProductionBody(rhs);

        return new ProductionRule(lhs, body);
    }

    private static Symbol[] ParseProductionBody(string body)
    {
        if(body.Length == 0)
        {
            throw new Exception("Production body cannot be empty.");
        }

        var symbols = new List<Symbol>();

        var alternatives = body.Split("|");

        if(alternatives.Length == 0)
        {
            alternatives = new string[] { body };
        }

        foreach (var alternative in alternatives)
        {
            if(alternative.Length == 0)
            {
                throw new Exception("Alternative cannot be empty.");
            }

            var segments = alternative.Split(",");

            if(segments.Length == 0)
            {
                segments = new string[] { alternative };
            }


        }

        return symbols.ToArray();
    }

    private static IEnumerable<Symbol> ParseAlternative(string alternative)
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize(alternative);
        var openOptionMacro = "{";
        var closeOptionMacro = "}";
        var openRepetitionMacro = "[";
        var closeRepetitionMacro = "]";
        var openGroupMacro = "(";
        var closeGroupMacro = ")";
        var pipeMacro = "|";

        foreach (var token in tokens)
        {
            if(token is null)
            {
                yield break;
            }

            if (token.Type == TokenType.Comment)
            {
                continue;
            }

            if (token.Type == TokenType.String)
            {
                yield return new Terminal(TokenType.Identifier, token.Value);
                continue;
            }

            if(token.Type == TokenType.Identifier)
            {
                yield return new NonTerminal(token.Value);
                continue;
            }

            if(token.Type != TokenType.Punctuation)
            {
                throw new Exception($"Unexpected token type: {token.Type}");
            }

            //switch (token.Value)
            //{
            //    case openOptionMacro:
            //        yield return new Option(ParseAlternative(alternative));
            //        break;
            //    case closeOptionMacro:
            //        break;
            //    case openRepetitionMacro:
            //        yield return new Repetition(ParseAlternative(alternative));
            //        break;
            //    case closeRepetitionMacro:
            //        break;
            //    case openGroupMacro:
            //        yield return new Group(ParseAlternative(alternative));
            //        break;
            //    case closeGroupMacro:
            //        break;
            //    case pipeMacro:
            //        break;

            //    default:
            //        throw new Exception($"Unexpected punctuation: {token.Value}");
            //}

        }
    }
}