//namespace TextAnalysis;


//public class ParserOutput
//{
//    public Symbol Symbol { get; }
//    public ISymbolTable SymbolTable { get; }
//    // to study.
//}

//public class Parser
//{
//    private RegularGrammar Grammar { get; }
//    private IParsingTable ParsingTable => Grammar.ParsingTable;

//    public Parser(RegularGrammar grammar)
//    {
//        Grammar = grammar;
//    }

//    public ParserOutput Parse(Lexer lexer)
//    {
//        throw new Exception();
//        var context = CreateContext(lexer);
//        var symbolTable = context.GetSymbolTable();
//        var stack = context.Stack;
//        var lookAhead = context.LookAhead;

//        AdvanceLookAhead(context);

//        while (stack.IsNotEmpty)
//        {
//            var stackSymbol = stack.Peek();

//            if (stackSymbol == null)
//            {
//                throw new Exception("Syntax error.");
//            }

//            if (stackSymbol.IsTerminal)
//            {
//                var lookAheadSymbol = lookAhead.First();

//                if (lookAheadSymbol == null)
//                {
//                    throw new Exception("Syntax error.");
//                }

//                if (!stackSymbol.Equals(lookAheadSymbol))
//                {
//                    throw new Exception("Syntax error.");
//                }

//                stack.Pop();
//                AdvanceLookAhead(context);
//                continue;
//            }

//            var production = ParsingTable.TryGetProduction(context);

//            if (production == null)
//            {
//                throw new Exception("Syntax error.");
//            }

//            var astNode = new TreeNode(production.Name, stackSymbol.ProductionName);

//            //var left = new NonTerminalSymbol("<integer>", new Symbol[]
//            //{
//            //    new TerminalSymbol("<digit>", "5"),
//            //    new TerminalSymbol("<digit>", "0"),
//            //});
//            //var right = new NonTerminalSymbol("<integer>", new Symbol[]
//            //{
//            //     new TerminalSymbol("<digit>", "3"),
//            //});
//            //var op = new TerminalSymbol("<operator>", "+");
//            //var terms = new Symbol[] { left, op, right };
//            //var expr = new NonTerminalSymbol("<expr>", terms);

//            stack.Pop();
//            stack.Push(production.Expand().Reverse());
//        }
//    }

//    private ParsingContext CreateContext(Lexer lexer)
//    {
//        var context = new ParsingContext(lexer);

//        context.Stack.Push(Grammar.InitialSymbol);
//        return context;
//    }

//    private void AdvanceLookAhead(ParsingContext context)
//    {
//        var lexer = context.Lexer;
//        var symbol = lexer.GetNextSymbol();

//        context.LookAhead.Set(symbol);
//    }
//}

//public class TreeNode
//{
//    public string Type { get; }
//    public string? Value { get; }
//    public TreeNode[] Children { get; }

//    public TreeNode(string type, string value, TreeNode[]? children = null)
//    {
//        Type = type;
//        Value = value;
//        Children = children ?? Array.Empty<TreeNode>();
//    }
//}