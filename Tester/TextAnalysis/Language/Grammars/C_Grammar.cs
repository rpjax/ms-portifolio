using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Grammars;

public class C_Grammar : GrammarDefinition
{
    public C_Grammar() : base(GetStart(), GetProductions())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // translation-unit:
            new ProductionRule("translation-unit", new NonTerminal("external-declaration")),
            
            //external-declaration:
            new ProductionRule("external-declaration", new NonTerminal("function-definition")),

            new ProductionRule("external-declaration", new NonTerminal("declaration")),

            //function-definition:
            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("declaration-list"), new NonTerminal("compound-statement")),

            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("compound-statement")),

            //declaration:
            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new NonTerminal("init-declarator-list"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("static-assert-declaration")),

            new ProductionRule("declaration", new Terminal(TokenType.Punctuation, ";")),

            //declaration-specifiers:
            new ProductionRule("declaration-specifiers", new NonTerminal("declaration-specifier"), new NonTerminal("declaration-specifiers")),

            new ProductionRule("declaration-specifiers", new Epsilon()),

            //declaration-specifier:
            new ProductionRule("declaration-specifier", new NonTerminal("storage-class-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-qualifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("function-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("alignment-specifier")),

        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("translation-unit");
    }
}
