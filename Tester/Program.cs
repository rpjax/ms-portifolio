using ModularSystem.Webql.Analysis.DocumentSyntax.Semantics.Components;
using ModularSystem.Webql.Analysis.Extensions;
using ModularSystem.Webql.Analysis.Parsing;
using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Semantics.Components;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.DocumentSyntax.Parsing;

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

public static class Program
{
    public static void Main()
    {
        Tokenizer3();
        var source = TestUser.Source.AsQueryable();

        var query = "[\r\n    [\r\n        \"source\"\r\n    ],\r\n    {\r\n        \"$filter\": [\r\n            \"filter_result\",\r\n            \"$source\",\r\n            [\r\n                [\r\n                    \"filter_item\"\r\n                ],\r\n                {\r\n                    \"$equals\": [\r\n                        null,\r\n                        \"$filter_item.nickname\",\r\n                        \"jacques\"\r\n                    ],\r\n                    \"$subtract\": [\r\n                        null,\r\n                        \"$filter_item.balance\",\r\n                        59\r\n                    ]\r\n                }\r\n            ]\r\n        ],\r\n        \"$select\": [\r\n            \"select_result\",\r\n            \"$source\",\r\n            [\r\n                [\r\n                    \"select_item\"\r\n                ],\r\n                {\r\n                    \"$subtract\": [\r\n                        null,\r\n                        \"$select_item.balance\",\r\n                        59\r\n                    ]\r\n                }\r\n            ]\r\n        ]\r\n    }\r\n]";
        var token = new DocumentSyntaxTokenizer()
            .Tokenize(query);

        var axiom = new AxiomParser()
            .ParseAxiom(new Webql.Analysis.Parsing.ParsingContext(), (ArrayToken)token);

        new RootLambdasArgumentTypeFixer(new Type[] { source.GetType() })
            .Execute(axiom);

        new LambdaArgumentTypeFixer()
            .Execute(axiom.Lambda!);

        var context = new AstSemanticAnalysis()
            .Execute(axiom);

        axiom = new MyRewriter(context)
            .Execute(axiom)
            .As<AxiomSymbol>();

        Console.WriteLine(axiom); ;
    }

    private static void Tokenizer3()
    {
        var analyser = new DocumentSyntaxTokenizer();

        var input = "{\r\n  \"filter\": \"foobar\",\r\n  \"select\": \"barbaz\",[\r\n}";

        var tokens = analyser.Tokenize(input)
            .ToArray()
            ;

        using var context = new  Webql.Analysis.DocumentSyntax.Parsing.ParsingContext(tokens);
        var block = BlockParser.ParseBlock(context);

        Environment.Exit(0);
    }
}

public class MyRewriter : AstSemanticRewriter
{
    public MyRewriter(SemanticContext context) : base(context)
    {
    }

    protected override void OnSemanticVisit(Symbol symbol)
    {
        if (symbol is IResultProducerOperatorExpressionSymbol resultProducer)
        {
            if (resultProducer.Destination is NullSymbol)
            {
                return;
            }
            if (resultProducer.Destination is not StringSymbol destination)
            {
                throw new Exception();
            }

            var resultExpression = resultProducer.As<ExpressionSymbol>();

            var resultProducerSemantic = SemanticAnalyser.AnalyseExpression(
                context: Context.GetSymbolContext(resultExpression),
                symbol: resultExpression
            );

            var declarationType = resultProducerSemantic.Type.AssemblyQualifiedName;
            var declarationIdentifier = destination.GetNormalizedValue();

            var declaration = new DeclarationStatementSymbol(
                type: declarationType,
                identifier: declarationIdentifier,
                modifiers: new[] { "cgen" },
                value: resultExpression
            );

            RewriteSymbol(resultExpression, declaration);
        }
    }
}