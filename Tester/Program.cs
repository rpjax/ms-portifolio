﻿using System.Diagnostics;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Grammars;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Debug;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Tools;

namespace ModularSystem.Tester;

public class TestUser
{
    public static List<TestUser> Source { get; } = new List<TestUser>()
    {
        new TestUser(){ Nickname = "Alice"},
        new TestUser(){ Nickname = "Bob"},
        new TestUser(){ Nickname = "Jacques"},
    };

    public string? Nickname { get; set; } = "";
    public decimal? Balance { get; set; }
}

/*
    Bro, listen:
    I'm falling asleep, I'm tired. The last conclusion i've reached is that the macros have to expand from the innermost to the outermost. From the leaves to the root. Each concrete macro must implement a method that expands itself. The expansion could be done with the help of a tree traversal algorithm. The tree traversal algorithm would require the base class Symbol to list it's children. At the moment this behavior does not exist. Also the tree traverser would need to have the hability to rewrite the tree. This is a very complex problem. I'm going to sleep now. I'm tired.
*/

public static class Program
{
    public static void Main()
    {
        //BenchmarkTokenizer();

        /*
         * NOTE: the tokenizer has been optimized for performance and its running very fast.    
         * The goal now is to implement the syntax analysis using a LL(1) parser. (top-down parser).
         * The first step is to implement a way of reading normal EBNF grammars and convert them to a format that can be used by the parser.
         * This envolves adjusing the production rules and the non-terminals to fix ambiguities and left-recursion.

         * GRAMMAR ANALYSIS WEBSITE: https://smlweb.cpsc.ucalgary.ca 
         * TAKE A LOOK AT THIS: https://en.wikipedia.org/wiki/Earley_parser

         */

        Grammar grammar = new GdefGrammar();

        var parser = new LR1Parser(grammar);
        var input = "foo : [ optional_bar ] baz ;";
        var gdefGrammarString = @"
grammar 
    : [ lexer_settings ] production_list 
    ;

lexer_settings
    : '<lexer>' { lexer_statement } '</lexer>'
    ;

lexer_statement
    : 'use' $id ';'
    | 'lexeme' $id regex ';'
    ;

regex
    : $string
    ;

production_list
    : production { production }
    ;

production
    : $id ':' production_body ';'
    ;

production_body
    : symbol { symbol } [ semantic_action ]
    ;

symbol
    : terminal
    | non_terminal
    | macro
    ;

terminal 
    : $string
    | lexeme
    | epsilon
    ;

non_terminal
    : $id
    ;

epsilon
    : 'ε'
    ;

macro
    : group
    | option
    | repetition
    | alternative
    ;

group
    : '(' symbol { symbol } ')'
    ;

option
    : '[' symbol { symbol } ']'
    ;

repetition
    : '{' symbol { symbol } '}'
    ;

alternative
    : '|'
    ;

lexeme
    : '$' $id 
    ;

semantic_action
    : ':' '{' '$' semantic_value '}'
    ;

semantic_value
    : '$' $int
    ;
";

        //* PARSER
        var cst = parser.Parse(gdefGrammarString);

        var whitelist = new string[] 
        {
            "production",
            "epsilon",
            "terminal",
            "lexeme",
            "non_terminal",
            "grouping",
            "option",
            "repetition",
            "alternative",
        };
        var reduced = new CstReducer(cst, whitelist)
            .ReduceRoot();
      
        return;
    }

    /*
     * Benchmark results:   
     * 
     * Total iterations: 100.000
     * Warm-up iterations: 1.000 (1%)
     * Tokens generated per iteration: 192
     * Average iteration time: 0,0000422951373737374 s (42 microseconds)
     * Worst time: 0,0788362 s
     * Best time: 0,0000379 s
     */
    private static void BenchmarkTokenizer()
    {
        var analyser = new Tokenizer();

        var input = "syntax         = { production } ;\r\nproduction     = identifier \"=\" expression \";\" ;\r\nexpression     = term { \"|\" term } ;\r\nterm           = factor { factor } ;\r\nfactor         = identifier\r\n               | literal\r\n               | \"[\" expression \"]\"     (* optional sequence *)\r\n               | \"{\" expression \"}\"     (* repetition *)\r\n               | \"(\" expression \")\"     (* grouping *) ;\r\nidentifier     = letter { letter | digit | \"_\" } ;\r\nliteral        = \"'\" character { character } \"'\" \r\n               | '\"' character { character } '\"' ;\r\nletter         = \"A\" | \"B\" | ... | \"Z\" | \"a\" | \"b\" | ... | \"z\" ;\r\ndigit          = \"0\" | \"1\" | ... | \"9\" ;\r\ncharacter      = letter | digit | symbol | escape ;\r\nsymbol         = \"[\" | \"]\" | \"{\" | \"}\" | \"(\" | \")\" | \"<\" | \">\" | \"'\" | '\"' | \"=\" | \"|\" | \".\" | \",\" | \";\" | \":\" ;\r\nescape         = \"\\\\\" ( [\"'\"] | [\"\\\"\"] | [\"n\"] | [\"t\"] | [\"\\\\\"] ) ;\r\n";

        var stopwatch = new Stopwatch();
        var times = new List<long>();
        var tokenCount = -1;
        var tokens = new Token?[0];

        for (int i = 0; i < 100000; i++)
        {
            stopwatch.Start();

            var _tokens = analyser.Tokenize(input)
                .ToArray();

            stopwatch.Stop();
            times.Add(stopwatch.ElapsedTicks);
            stopwatch.Reset();

            tokenCount = _tokens.Length;
            tokens = _tokens;
        }

        //* Skip the warp-up iterations.
        times = times.Skip(1000).ToList();

        var totalTime = times.Sum() / (double)Stopwatch.Frequency;
        var averageTime = times.Average() / Stopwatch.Frequency;
        var worstTime = times.Max() / (double)Stopwatch.Frequency;
        var bestTime = times.Min() / (double)Stopwatch.Frequency;

        Console.WriteLine($"Tokens generated per iteration: {tokenCount}");
        Console.WriteLine($"Average iteration time: {ToNonScientificString(averageTime)} s");
        Console.WriteLine($"Worst time: {ToNonScientificString(worstTime)} s");
        Console.WriteLine($"Best time: {ToNonScientificString(bestTime)} s");
        Console.WriteLine();

        foreach (var item in tokens)
        {
            Console.WriteLine(item);
        }
    }

    /*
     * Benchmark results for LR(1) parser to parse the gdef grammar using itself:   
     * 
     * Total iterations: 100.000
     * Warm-up iterations: 1.000 (1%)
     * Average iteration time: 0,000625697309090909 s (625 microseconds)
     * Worst time: 0,0171312 s
     * Best time: 0,000567 s
     */
    private static void BenchmarkLR1Parser()
    {
        var input = @"
grammar 
    : [ lexer_settings ] production_list 
    ;

lexer_settings
    : '<lexer>' { lexer_statement } '</lexer>'
    ;

lexer_statement
    : 'use' $id ';'
    | 'lexeme' $id regex ';'
    ;

regex
    : $string
    ;

production_list
    : production { production }
    ;

production
    : $id ':' production_body ';'
    ;

production_body
    : symbol { symbol } [ semantic_action ]
    ;

symbol
    : terminal
    | non_terminal
    | macro
    ;

terminal 
    : $string
    | lexeme
    | epsilon
    ;

non_terminal
    : $id
    ;

epsilon
    : 'ε'
    ;

macro
    : group
    | option
    | repetition
    | alternative
    ;

group
    : '(' symbol { symbol } ')'
    ;

option
    : '[' symbol { symbol } ']'
    ;

repetition
    : '{' symbol { symbol } '}'
    ;

alternative
    : '|'
    ;

lexeme
    : '$' $id 
    ;

semantic_action
    : ':' '{' '$' semantic_value '}'
    ;

semantic_value
    : '$' $int
    ;
";

        var stopwatch = new Stopwatch();
        var times = new List<long>();

        var parser = new LR1Parser(new GdefGrammar());

        for (int i = 0; i < 100000; i++)
        {
            stopwatch.Start();

            _ = parser.Parse(input);

            stopwatch.Stop();
            times.Add(stopwatch.ElapsedTicks);
            stopwatch.Reset();
        }

        //* Skip the warp-up iterations.
        times = times.Skip(1000).ToList();

        var totalTime = times.Sum() / (double)Stopwatch.Frequency;
        var averageTime = times.Average() / Stopwatch.Frequency;
        var worstTime = times.Max() / (double)Stopwatch.Frequency;
        var bestTime = times.Min() / (double)Stopwatch.Frequency;

        Console.WriteLine($"Average iteration time: {ToNonScientificString(averageTime)} s");
        Console.WriteLine($"Worst time: {ToNonScientificString(worstTime)} s");
        Console.WriteLine($"Best time: {ToNonScientificString(bestTime)} s");
        Console.WriteLine($"Total elaped time: {ToNonScientificString(totalTime)} s");
        Console.WriteLine();
    }

    private static string ToNonScientificString(double value)
    {
        return value.ToString("#0." + new string('#', 339));
    }

    // private static void TestSyntesis()
    // {
    //     var source = TestUser.Source.AsQueryable();

    //     var query = "[\r\n    [\r\n        \"source\"\r\n    ],\r\n    {\r\n        \"$filter\": [\r\n            \"filter_result\",\r\n            \"$source\",\r\n            [\r\n                [\r\n                    \"filter_item\"\r\n                ],\r\n                {\r\n                    \"$equals\": [\r\n                        null,\r\n                        \"$filter_item.nickname\",\r\n                        \"jacques\"\r\n                    ],\r\n                    \"$subtract\": [\r\n                        null,\r\n                        \"$filter_item.balance\",\r\n                        59\r\n                    ]\r\n                }\r\n            ]\r\n        ],\r\n        \"$select\": [\r\n            \"select_result\",\r\n            \"$source\",\r\n            [\r\n                [\r\n                    \"select_item\"\r\n                ],\r\n                {\r\n                    \"$subtract\": [\r\n                        null,\r\n                        \"$select_item.balance\",\r\n                        59\r\n                    ]\r\n                }\r\n            ]\r\n        ]\r\n    }\r\n]";
    //     var token = new DocumentSyntaxTokenizer()
    //         .Tokenize(query);

    //     var axiom = new AxiomParser()
    //         .ParseAxiom(new Webql.Analysis.Parsing.ParsingContext(), (ArrayToken)token);

    //     new RootLambdasArgumentTypeFixer(new Type[] { source.GetType() })
    //         .Execute(axiom);

    //     new LambdaArgumentTypeFixer()
    //         .Execute(axiom.Lambda!);

    //     var context = new AstSemanticAnalysis()
    //         .Execute(axiom);

    //     axiom = new MyRewriter(context)
    //         .Execute(axiom)
    //         .As<AxiomSymbol>();

    //     Console.WriteLine(axiom); ;
    // }

}

// public class MyRewriter : AstSemanticRewriter
// {
//     public MyRewriter(SemanticContext context) : base(context)
//     {
//     }

//     protected override void OnSemanticVisit(Webql.Analysis.Symbols.Symbol symbol)
//     {
//         if (symbol is IResultProducerOperatorExpressionSymbol resultProducer)
//         {
//             if (resultProducer.Destination is NullSymbol)
//             {
//                 return;
//             }
//             if (resultProducer.Destination is not StringSymbol destination)
//             {
//                 throw new Exception();
//             }

//             var resultExpression = resultProducer.As<ExpressionSymbol>();

//             var resultProducerSemantic = SemanticAnalyser.AnalyseExpression(
//                 context: Context.GetSymbolContext(resultExpression),
//                 symbol: resultExpression
//             );

//             var declarationType = resultProducerSemantic.Type.AssemblyQualifiedName;
//             var declarationIdentifier = destination.GetNormalizedValue();

//             var declaration = new DeclarationStatementSymbol(
//                 type: declarationType,
//                 identifier: declarationIdentifier,
//                 modifiers: new[] { "cgen" },
//                 value: resultExpression
//             );

//             RewriteSymbol(resultExpression, declaration);
//         }
//     }
// }

