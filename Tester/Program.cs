using ModularSystem.Webql.Analysis.Parsing;
using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Semantics.Visitors;
using ModularSystem.Webql.Analysis.Tokenization;
using ModularSystem.Webql.Analysis.Tokens;

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
        var source = TestUser.Source.AsQueryable();

        var query = "[\r\n    [\r\n        \"source\"\r\n    ],\r\n    {\r\n        \"$filter\": [\r\n            \"result\",\r\n            \"$source\",\r\n            [\r\n                [\r\n                    \"item\"\r\n                ],\r\n                {\r\n                    \"$equals\": [\r\n                        null,\r\n                        \"$item.nickname\",\r\n                        \"jacques\"\r\n                    ],\r\n                    \"$subtract\": [\r\n                        null,\r\n                        \"$item.balance\",\r\n                        59\r\n                    ]\r\n                }\r\n            ]\r\n        ]\r\n    }\r\n]";
        var token = new LexicalAnalyser()
            .Tokenize(query);

        var axiom = new AxiomParser()
            .ParseAxiom(new ParsingContext(), (ArrayToken)token);

        var context = new SemanticsAnalysisVisitor()
            .Run(axiom, new Type[] { source.GetType() });   

        Console.WriteLine(axiom); ;
    }
}