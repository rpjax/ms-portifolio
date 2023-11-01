//namespace TextAnalysis;

//public class ParsingContext
//{
//    public SymbolStack Stack { get; }
//    public SymbolLookAhead LookAhead { get; }
//    public Lexer Lexer { get; }

//    private TreeNode? Ast { get; set; }

//    public ParsingContext(Lexer lexer)
//    {
//        Stack = new();
//        LookAhead = new();
//        Lexer = lexer;
//        Ast = null;
//    }

//    public ISymbolTable GetSymbolTable()
//    {
//        return Lexer.GetSymbolTable();
//    }
//}

//public class SymbolStack
//{
//    public bool IsEmpty => Stack.Count == 0;
//    public bool IsNotEmpty => Stack.Count > 0;
//    public Symbol? CurrentSymbol => Peek();

//    private Stack<Symbol> Stack { get; } = new();

//    public SymbolStack() { }

//    public SymbolStack Push(Symbol symbol)
//    {
//        Stack.Push(symbol);
//        return this;
//    }

//    public SymbolStack Push(IEnumerable<Symbol> symbols)
//    {
//        foreach (var item in symbols)
//        {
//            Push(item);
//        }

//        return this;
//    }

//    public Symbol Pop()
//    {
//        return Stack.Pop();
//    }

//    public Symbol? Peek()
//    {
//        if (Stack.Count == 0)
//        {
//            return null;
//        }

//        return Stack.Peek();
//    }

//}

//public class SymbolLookAhead
//{
//    public bool IsEmpty => Symbols == null || Symbols.Length == 0;
//    public bool IsNotEmpty => Symbols?.Length > 0;

//    private Symbol[]? Symbols { get; set; } = null;

//    public SymbolLookAhead() { }

//    public override string? ToString()
//    {
//        //returns literal value: ex "a", "b" etc...
//        if (Symbols == null)
//        {
//            return null;
//        }

//        return string.Join("", Symbols.Select((symbol) => symbol.ToString()));
//    }

//    public Symbol? First()
//    {
//        if (IsEmpty)
//        {
//            return null;
//        }

//        return Symbols![0];
//    }

//    public ReadOnlySpan<Symbol> GetSymbols()
//    {
//        return Symbols;
//    }

//    public void Set(params Symbol[]? symbols)
//    {
//        Symbols = symbols;
//    }

//    public void Set(Symbol? symbol)
//    {
//        if(symbol == null)
//        {
//            Symbols = null;
//            return;
//        }

//        Symbols = new[] { symbol };
//    }
//}