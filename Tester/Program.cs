using System.Diagnostics;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Grammars;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Debug;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Parsing;

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

        var grammar = new LR1TestGrammar2();
        var parser = new LR1Parser(grammar);
        var input = "// <lexer>\r\n\r\n// use standard;\r\n// use csharp;\r\n\r\n// lexeme hex_number '[0-9a-fA-F]+' ;\r\n// lexeme string '\"[^\"]*\"|\\'[^\\']*\\'' ;\r\n\r\n// </lexer>\r\n\r\ngrammar \r\n    : [ lexer_settings ] production_list \r\n    ;\r\n\r\nlexer_settings\r\n    : '<lexer>' { lexer_statement } '</lexer>'\r\n    ;\r\n\r\nlexer_statement\r\n    : 'use' $id ';'\r\n    | 'lexeme' $id regex ';'\r\n    ;\r\n\r\nregex\r\n    : $string\r\n    ;\r\n\r\nproduction_list\r\n    : production { production }\r\n    ;\r\n\r\nproduction\r\n    : $id ':' production_body ';'\r\n    ;\r\n\r\nproduction_body\r\n    : symbol { symbol } [ semantic_action ]\r\n    ;\r\n\r\nsymbol\r\n    : terminal\r\n    | non_terminal\r\n    | macro\r\n    ;\r\n\r\nterminal \r\n    : $string\r\n    | lexeme\r\n    | epsilon\r\n    ;\r\n\r\nnon_terminal\r\n    : $id\r\n    ;\r\n\r\nepsilon\r\n    : 'ε'\r\n    ;\r\n\r\nmacro\r\n    : group\r\n    | option\r\n    | repetition\r\n    | alternative\r\n    ;\r\n\r\ngroup\r\n    : '(' symbol { symbol } ')'\r\n    ;\r\n\r\noption\r\n    : '[' symbol { symbol } ']'\r\n    ;\r\n\r\nrepetition\r\n    : '{' symbol { symbol } '}'\r\n    ;\r\n\r\nalternative\r\n    : '|'\r\n    ;\r\n\r\nlexeme\r\n    : '$' $id \r\n    ;\r\n\r\nsemantic_action\r\n    : '{' '$' semantic_value '}'\r\n    ;\r\n\r\nsemantic_value\r\n    : '$' $int\r\n    ;";

        var cst = parser.Parse("5 + 3 * 2");
      
        Console.WriteLine("Original grammar:");
        Console.WriteLine(grammar.GetOriginalGrammar());
        Console.WriteLine("Transformations:");
        Console.WriteLine(grammar.Productions.TransformationCollection);
        Console.WriteLine("Final grammar:");
        Console.WriteLine(grammar.Productions);

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

