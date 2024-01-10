using Core.TextAnalysis;

namespace Domain;

public static class Prods
{
    //*
    // <whitespace> ::= " "
    //*
    public class WhiteSpaceProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new string[] { " " };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<whitespace>");
        }
    }

    //*
    // <digit> ::= "0" | "1" | ... | "9"
    //*
    public class DigitProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<digit>", sentense.GetTokens());
        }

    }

    //*
    // <integer> ::= <digit> | <integer> <digit> 
    //*
    public class IntegerProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new[]
            {
                "<digit>",
                "<integer><digit>",
            };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<integer>", sentense.GetTokens());
        }
    }

    //*
    // <float> ::= <integer> "." <integer> | <float> <integer>
    //*
    public class FloatProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new[]
            {
                "<integer>.<integer>",
                "<float>.<integer>",
            };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<float>", sentense.GetTokens());
        }
    }

    //*
    // <number> ::= <integer> | <number> <integer> | <float> 
    //*
    public class NumberProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new[]
            {
                "<integer>",
                "<float>",
                "<number><integer>"
            };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<number>", sentense.GetTokens());
        }
    }

    //*
    // <operator> ::= "+" | "-" | "*" | "/"
    //*
    public class OperatorProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new[] { "+", "-", "*", "/" };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<operator>", sentense.GetTokens());
        }
    }

    //*
    // <expr> ::= <number> <operator> <number> | <expr> <operator> <number> | <expr> <operator> <expr> 
    //*
    public class ExprProduction : TokenProduction
    {
        public override string[] GetFormationStrings()
        {
            return new string[]
            {
                "<number><operator><number>",
                "<expr><operator><number>",
                "<expr><operator><expr>",
            };
        }

        public override LexerToken GetToken(LexerSentense sentense)
        {
            return new RawToken("<expr>", sentense.GetTokens());
        }
    }
}
