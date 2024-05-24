using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Gdef;

public class GdefGrammarBuilder
{
    private CstRoot Root { get; }

    public GdefGrammarBuilder(CstRoot root)
    {
        Root = root;
    }

    public Grammar Build()
    {
        var start = GetStart(Root);
        var productions = GetProductions(Root);

        return new Grammar(start, productions);
    }

    private NonTerminal GetStart(CstRoot root)
    {
        return GetProductions(root).First().Head;
    }

    private IEnumerable<ProductionRule> GetProductions(CstRoot root)
    {
        var productionNodes = root.Children
            .Where(x => x.Type == CstNodeType.Internal)
            .Select(x => (CstInternal)x);

        return productionNodes
            .Select(BuildProduction);
    }

    private ProductionRule BuildProduction(CstInternal node)
    {
        if(node.Symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException();
        }

        if (nonTerminal.Name != "production")
        {
            throw new Exception();
        }
        if (node.Children.Length < 4)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children;

        if (children[0] is not CstLeaf leaf)
        {
            throw new InvalidOperationException();
        }

        if (leaf.Symbol is not Terminal terminal)
        {
            throw new InvalidOperationException();
        }

        if (terminal.Type != TokenType.Identifier)
        {
            throw new InvalidOperationException();
        }
        if (terminal.Value is null)
        {
            throw new InvalidOperationException();
        }

        var head = new NonTerminal(terminal.Value);

        var bodyNodes = children
            .Skip(2)
            .Take(children.Length - 3)
            .ToArray();

        var body = TranslateSentence(bodyNodes);

        return new ProductionRule(head, body);
    }

    private Sentence TranslateSentence(CstNode[] nodes)
    {
        var symbols = nodes
            .Where(x => x is CstLeaf leaf ? !leaf.IsEpsilon : true)
            .SelectMany(x => TranslateNode(x))
            .ToArray();

        return new Sentence(symbols);
    }

    private IEnumerable<Symbol> TranslateNode(CstNode node)
    {
        switch (node.Type)
        {
            case CstNodeType.Internal:
                return TranslateInternalNode((CstInternal)node);

            case CstNodeType.Leaf:
                return TranslateLeafNode((CstLeaf)node);

            default:
                throw new InvalidOperationException();
        }
    }

    private IEnumerable<Symbol> TranslateInternalNode(CstInternal node)
    {
        if (node.Symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException();
        }

        var isLexeme = nonTerminal.Name == "lexeme";

        if(isLexeme)
        {
            yield return TranslateLexeme(node, nonTerminal);
        }
        else
        {
            yield return TranslateMacro(node, nonTerminal);
        }
    }

    private Symbol TranslateLexeme(CstInternal node, NonTerminal nonTerminal)
    {
        if(node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeaf leaf)
        {
            throw new InvalidOperationException();
        }

        if(leaf.Symbol is not Terminal terminal)
        {
            throw new InvalidOperationException();
        }
        if(terminal.Value is null)
        {
            throw new InvalidOperationException();
        }

        var lexemeType = GetLexemeType(terminal.Value);

        return new Terminal(lexemeType, null);
    }

    private TokenType GetLexemeType(string str)
    {
        switch (str)
        {
            case "id":
                return TokenType.Identifier;

            case "string":
                return TokenType.String;

            case "int":
                return TokenType.Integer;

            case "float":
                return TokenType.Float;

            case "hex":
                return TokenType.Hexadecimal;

            default:
                throw new InvalidOperationException();
        }
    }

    private Symbol TranslateMacro(CstInternal node, NonTerminal nonTerminal)
    {
        var macroType = GetMacroType(nonTerminal.Name);
        var macroSentence = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray()
            ;

        switch (macroType)
        {
            case MacroType.Grouping:
                
                return new GroupingMacro(TranslateSentence(macroSentence));

            case MacroType.Option:
                return new OptionMacro(TranslateSentence(macroSentence));

            case MacroType.Repetition:
                return new RepetitionMacro(TranslateSentence(macroSentence));

            case MacroType.Alternative:
                return new AlternativeMacro();

            default:
                throw new InvalidOperationException();
        }
    }

    private IEnumerable<Symbol> TranslateLeafNode(CstLeaf node)
    {
        if (node.Symbol is not Terminal terminal)
        {
            throw new InvalidOperationException();
        }
        if(terminal.Value is null)
        {
            throw new InvalidOperationException();
        }

        switch (terminal.Type)
        {
            //* case a non-terminal identifier
            case TokenType.Identifier:
                yield return new NonTerminal(terminal.Value);
                break;

            //* case a terminal literal
            case TokenType.String:
                var normalizedValue = terminal.GetNormalizedValue();
                var tokens = Tokenizer.Instance.Tokenize(normalizedValue, includeEoi: false);

                foreach (var token in tokens)
                {
                    yield return new Terminal(token.Type, token.Value);
                }
                break;

            default:
                yield return terminal;
                yield break;
        }
    }

    private MacroType GetMacroType(string str)
    {
        switch (str)
        {
            case "grouping":
                return MacroType.Grouping;

            case "option":
                return MacroType.Option;

            case "repetition":
                return MacroType.Repetition;

            case "alternative":
                return MacroType.Alternative;

            default:
                throw new InvalidOperationException();
        }
    }

}