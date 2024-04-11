namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class InitialState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(this, TokenizerAction.End);
        }

        var isWhitespace = char.IsWhiteSpace(c.Value);

        if(isWhitespace)
        {
            return new TransitionResult(this, TokenizerAction.Skip);
        }

        var isDigit = char.IsDigit(c.Value);

        if (isDigit)
        {
            return new TransitionResult(new IntegerNumberState(), TokenizerAction.Read);
        }

        var isLetter = char.IsLetter(c.Value);
        var isUnderline = c == LexicalAlphabet.Underline;

        if (isLetter || isUnderline)
        {
            return new TransitionResult(new IdentifierOrKeywordState(c.Value), TokenizerAction.Read);
        }

        var isStringDelimiter = LexicalAlphabet.StringDelimiters.Contains(c.Value);

        if (isStringDelimiter)
        {
            return new TransitionResult(new StringState(c.Value), TokenizerAction.Read);
        }

        var isPunctuation = LexicalAlphabet.Punctuations
            .Contains(c.Value);

        if (isPunctuation)
        {
            return new TransitionResult(new PunctuationState(), TokenizerAction.Read);
        }

        var isOperator = LexicalAlphabet.Operators
            .Select(x => x.First())
            .Contains(c.Value);

        if(isOperator)
        {
            return new TransitionResult(new OperatorState(c.Value), TokenizerAction.Read);
        }

        return new TransitionResult(this, TokenizerAction.Error);
    }
}
