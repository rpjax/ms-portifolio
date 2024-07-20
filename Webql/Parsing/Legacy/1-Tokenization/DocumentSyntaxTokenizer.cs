using ModularSystem.TextAnalysis.Tokenization;

namespace ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;


/*
 * The Tokenizer class works well, it's simple and easy to understand. However, it does not perform so well. 
 * The above code is a refactored version of the Tokenizer class. It uses hashsets to optimeze the tokenization process. 
 * The state is separated from the machine, the state contains everything that is needed to keep track of the current state of the machine, the machine contains the constant logic of the machine. 
 */

public class TokenizerState
{
    public string CurrentState { get; set; } = "";
}

public class TokenizerTransition
{
    public char Character { get; set; }
    public string NextState { get; }
}

//public class TokenizerMachine
//{
//    private TokenizerState TargetState { get; } = new TokenizerState();

//    public IEnumerable<Token> Tokenize(IEnumerable<char> source)
//    {
//        var token = new StringBuilder();

//        foreach (var character in source)
//        {
//            token.Append(character);
//        }

//        yield return new Token(ExpressionType.String, token.ToString());
//    }

//    private void TransitionState(char character)
//    {
//        TargetState.CurrentState += character;
//    }

//    private void
//}
