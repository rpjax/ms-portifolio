using ModularSystem.Webql.Analysis.Parsing;
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

    public string Nickname { get; set; } = "";
}

public static class Program
{
    public static void Main()
    {
        var source = TestUser.Source.AsQueryable();

        var query = "[\r\n  [\r\n    \"source\"\r\n  ],\r\n  {\r\n    \"$filter\": [\r\n      \"result\",\r\n      \"$source\",\r\n      [\r\n        [\r\n          \"item\"\r\n        ],\r\n        {\r\n          \"$add\": [\r\n            \"addResult\",\r\n            \"$item.value\",\r\n            1\r\n          ]\r\n        }\r\n      ]\r\n    ]\r\n  }\r\n]";
        var token = new LexicalAnalyser()
            .Tokenize(query);

        var axiom = new AxiomParser()
            .ParseAxiom(new ParsingContext(), (ArrayToken)token);

        new RootLambdasArgumentTypeFixer(new Type[] { source.GetType() })
            .Execute(axiom);

        new LambdaArgumentTypeFixer()
            .Execute(axiom.Lambda);

        new SemanticsAnalysisVisitor()
            .Run(axiom);

        Console.WriteLine(axiom); ;
    }
}