using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using ModularSystem.Core.Linq;
using ModularSystem.Core.TextAnalysis.Tokenization;
using ModularSystem.Core.TextAnalysis.Grammars;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using Webql.Parsing;
using Webql.Semantics;
using Webql.Semantics.Extensions;
using Webql.Core;
using Webql;
using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Tester;

/*
 * Debate com o mano lá
 */

//public static int ComputeHashUsingMutableVariable(string value)
//{
//    int hash = 16777619;

//    foreach (char c in value)
//    {
//        hash = (hash * 31) ^ c;
//    }
//    return hash;
//}

//public static int ComputeHashUsingRecursion(string value, bool isRecursiveCall)
//{
//    if (value.Length == 0)
//    {
//        return -1;
//    }

//    var c = value[0];
//    var baseHash = 16777619;

//    if(isRecursiveCall)
//    {
//        return (baseHash * 31) ^ c;
//    }
//}

//public static int ComputeHashUsingStack(string value)
//{
//    var baseHash = 16777619;
//    var stack = new Stack<int>();

//    stack.Push(baseHash);

//    foreach (char c in value)
//    {
//        var hash = stack.Pop();
//        var newHash = (hash * 31) ^ c;

//        stack.Push(newHash);
//    }

//    return stack.Pop();
//}

public class TestWallet
{
    public decimal UsdBalance { get; set; }
    public decimal UsdtBalance { get; set; }
}

public class TestUser
{
    public static List<TestUser> Source { get; } = new List<TestUser>()
    {
        new(){ Nickname = "Alice", IsActive = true },
        new(){ Nickname = "Bob", IsActive = true },
        new(){ Nickname = "Jacques", IsActive = true, Wallet = new(){ UsdBalance = 100 } },
    };

    public string? Nickname { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public TestWallet Wallet { get; set; } = new TestWallet();
    public Identity Identity { get; set; } = new Identity(
        uniqueIdentifier: "jacques",
        permissions: Enumerable.Empty<IdentityPermission>(),
        roles: new[] { "admin" }
    );
}

public static class Program
{
    public static void Main()
    {
        var settings = new WebqlCompilerSettings(
            queryableType: typeof(IQueryable<>),
            entityType: typeof(TestUser)
        );

        var compiler = new WebqlCompiler(settings);

        var query = @"
        { 
            $filter: { 
                //isActive: true,
                //nickname: { $like: 'jacques' },
                //email: { $not: null },
                //wallet: { 
                //    usdBalance: { 
                //        $greater: 59 
                //    } 
                //},
                identity: {
                    roles: {
                        $aggregate: {
                            $filter: { $equals: 'admin' },
                            $count: { },
                            $greater: 0
                        }
                    }
                }
            } 
        }";

        var expression = compiler.Translate(query);

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
        var Analyzer = new Tokenizer();

        var input = "syntax         = { production } ;\r\nproduction     = identifier \"=\" expression \";\" ;\r\nexpression     = term { \"|\" term } ;\r\nterm           = factor { factor } ;\r\nfactor         = identifier\r\n               | literal\r\n               | \"[\" expression \"]\"     (* optional sequence *)\r\n               | \"{\" expression \"}\"     (* repetition *)\r\n               | \"(\" expression \")\"     (* grouping *) ;\r\nidentifier     = letter { letter | digit | \"_\" } ;\r\nliteral        = \"'\" character { character } \"'\" \r\n               | '\"' character { character } '\"' ;\r\nletter         = \"A\" | \"B\" | ... | \"Z\" | \"a\" | \"b\" | ... | \"z\" ;\r\ndigit          = \"0\" | \"1\" | ... | \"9\" ;\r\ncharacter      = letter | digit | symbol | escape ;\r\nsymbol         = \"[\" | \"]\" | \"{\" | \"}\" | \"(\" | \")\" | \"<\" | \">\" | \"'\" | '\"' | \"=\" | \"|\" | \".\" | \",\" | \";\" | \":\" ;\r\nescape         = \"\\\\\" ( [\"'\"] | [\"\\\"\"] | [\"n\"] | [\"t\"] | [\"\\\\\"] ) ;\r\n";

        var stopwatch = new Stopwatch();
        var times = new List<long>();
        var tokenCount = -1;
        var tokens = new Token?[0];

        for (int i = 0; i < 100_000; i++)
        {
            stopwatch.Start();

            var _tokens = Analyzer.Tokenize(input)
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

    private static void BenchmarkRoslynTokenizer()
    {
        var input = "syntax         = { production } ;\r\nproduction     = identifier \"=\" expression \";\" ;\r\nexpression     = term { \"|\" term } ;\r\nterm           = factor { factor } ;\r\nfactor         = identifier\r\n               | literal\r\n               | \"[\" expression \"]\"     (* optional sequence *)\r\n               | \"{\" expression \"}\"     (* repetition *)\r\n               | \"(\" expression \")\"     (* grouping *) ;\r\nidentifier     = letter { letter | digit | \"_\" } ;\r\nliteral        = \"'\" character { character } \"'\" \r\n               | '\"' character { character } '\"' ;\r\nletter         = \"A\" | \"B\" | ... | \"Z\" | \"a\" | \"b\" | ... | \"z\" ;\r\ndigit          = \"0\" | \"1\" | ... | \"9\" ;\r\ncharacter      = letter | digit | symbol | escape ;\r\nsymbol         = \"[\" | \"]\" | \"{\" | \"}\" | \"(\" | \")\" | \"<\" | \">\" | \"'\" | '\"' | \"=\" | \"|\" | \".\" | \",\" | \";\" | \":\" ;\r\nescape         = \"\\\\\" ( [\"'\"] | [\"\\\"\"] | [\"n\"] | [\"t\"] | [\"\\\\\"] ) ;\r\n";

        var stopwatch = new Stopwatch();
        var times = new List<long>();
        var tokenCount = -1;

        for (int i = 0; i < 100_000; i++)
        {
            stopwatch.Start();

            var _tokens = SyntaxFactory.ParseTokens(input)
                .ToArray();

            stopwatch.Stop();
            times.Add(stopwatch.ElapsedTicks);
            stopwatch.Reset();

            tokenCount = _tokens.Length;
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
    }

    /*
     * Benchmark results for LR(1) parser, parsing the gdef grammar using itself:   
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
        var root = null as CstRoot;

        for (int i = 0; i < 10_000; i++)
        {
            stopwatch.Start();

            root = parser.Parse(input);

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
        Console.WriteLine(root?.ToHtmlTreeView());
    }

    /*
     * Benchmark results for WebQL Document-Syntax LR(1) parser:   
     * 
     * Total iterations: 100.000
     * Warm-up iterations: 1.000 (1%)
     * Average iteration time: 0,0000653526434343434 s (65 microseconds)
     * Worst time: 0,00399 s
     * Best time: 0,0000616 s
     * Total elaped time: 6,4699117 s
     */
    private static void BenchmarkWebqlParser()
    {
        var input = @"
            { 
                $filter: { 
                    isActive: true,
                    nickname: { $like: 'jacques' },
                    balance: { $greater: 59 }
                } 
            }";

        //input = "{ where: { eventId: { $equals: 402 } } }";

        var stopwatch = new Stopwatch();
        var times = new List<long>();

        var parser = WebqlParser.GetParser();

        for (int i = 0; i < 100_000; i++)
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

    //     new RootLambdasArgumentTypeFixer(new ExpressionType[] { source.GetType() })
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
//     public MyRewriter(SemanticContextOld context) : base(context)
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

//             var resultProducerSemantic = SemanticAnalyzer.CreateExpressionSymbol(
//                 context: Context.GetSymbolContext(resultExpression),
//                 symbol: resultExpression
//             );

//             var declarationType = resultProducerSemantic.ExpressionType.AssemblyQualifiedName;
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

