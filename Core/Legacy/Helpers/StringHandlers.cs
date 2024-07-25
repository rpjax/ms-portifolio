using System.Text.RegularExpressions;

namespace ModularSystem.Core.Helpers;

/// <summary>
/// An API to work with strings.
/// </summary>
public class StringHandler
{
    public delegate void StopDelegate();

    public class Sequence
    {
        public int StartIndex { get; set; } = 0;
        public int EndIndex { get; set; } = 0;
        public int Length => Chars.Count;
        public List<char> Chars { get; set; } = new();

        public Sequence() { }

        public override string ToString()
        {
            return new string(Chars.ToArray());
        }

        public bool Equals(Sequence other)
        {
            return StartIndex == other.StartIndex && EndIndex == other.EndIndex
               && Chars.SequenceEqual(other.Chars);
        }

        public Sequence DeepCopy()
        {
            return new Sequence()
            {
                StartIndex = StartIndex,
                EndIndex = EndIndex,
                Chars = new List<char>(Chars)
            };
        }

        public bool StartsWith(string str)
        {
            if (ToString().StartsWith(str))
            {
                return true;
            }

            return false;
        }
    }

    public class CutValue
    {
        public string Left;
        public string Right;
        public Sequence CuttedSequence;

        public CutValue(string left, string right, Sequence cuttedSequence)
        {
            Left = left;
            Right = right;
            CuttedSequence = cuttedSequence;
        }

        public string ToStringWihoutCuttedValue()
        {
            return Left + Right;
        }
    }

    readonly string originalStr;
    string str;

    public StringHandler(string str)
    {
        originalStr = str;
        this.str = str;

        if (str.Length == 0)
        {
            throw new InvalidOperationException("StringHandler cannot operate with a string of zero length.");
        }
    }

    public static StringHandler? From(string str)
    {
        if (str.IsEmpty())
        {
            return null;
        }

        return new StringHandler(str);
    }

    public string GetOriginalString()
    {
        return originalStr;
    }

    public int Iterate(Action<char, StopDelegate> lambda, int startIndex = 0)
    {
        bool stop = false;
        int index = str.Length - 1;
        StopDelegate stopFunc = () => stop = true;

        if (startIndex >= str.Length)
        {
            throw new InvalidOperationException("Start index cannot be greater equal the size of the string iterated on.");
        }

        for (int i = startIndex; i < str.Length; i++)
        {
            lambda(str[i], stopFunc);

            if (stop)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public int Iterate(Action<char, int, StopDelegate> lambda, int startIndex = 0)
    {
        bool stop = false;
        int index = str.Length - 1;
        StopDelegate stopFunc = () => stop = true;

        if (startIndex >= str.Length)
        {
            throw new InvalidOperationException("Start index cannot be greater equal the size of the string iterated on.");
        }

        for (int i = 0; i < str.Length; i++)
        {
            lambda(str[i], i, stopFunc);

            if (stop)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public int IterateBackwards(Action<char, StopDelegate> lambda, int startIndex = 0)
    {
        bool stop = false;
        int index = 0;
        StopDelegate stopFunc = () => stop = true;

        if (startIndex >= str.Length)
        {
            throw new InvalidOperationException("Start index cannot be greater equal the size of the string iterated on.");
        }

        for (int i = str.Length - 1; i >= 0; i--)
        {
            lambda(str[i], stopFunc);

            if (stop)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public int IterateBackwards(Action<char, int, StopDelegate> lambda, int startIndex = 0)
    {
        bool stop = false;
        int index = 0;
        StopDelegate stopFunc = () => stop = true;

        if (startIndex >= str.Length)
        {
            throw new InvalidOperationException("Start index cannot be greater equal the size of the string iterated on.");
        }

        for (int i = str.Length - 1; i >= 0; i--)
        {
            lambda(str[i], i, stopFunc);

            if (stop)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public int IterateUntilCharacter(char character)
    {
        return Iterate((x, stop) =>
        {
            if (x == character)
            {
                stop();
            }
        });
    }

    public int IterateUntilSequentialCharacter(char character, int occurrences)
    {
        return Iterate((x, stop) =>
        {
            if (x == character)
            {
                occurrences--;
            }
            if (occurrences == 0)
            {
                stop();
            }
        });
    }

    public List<Sequence> BreakIntoSequences(int sequenceLength)
    {
        var sequences = new List<Sequence>();

        Iterate((x, index, stop) =>
        {
            var length = sequenceLength;

            if (index + length >= str.Length)
            {
                stop();
            }

            var sample = str.Substring(index, length);
            var sequence = new Sequence()
            {
                StartIndex = index,
                EndIndex = index + length,
                Chars = sample.ToArray().ToList()
            };
            sequences.Add(sequence);
        });

        return sequences;
    }

    public List<Sequence> BreakIntoSequences(Regex pattern)
    {
        var sequences = new List<Sequence>();
        var matches = pattern.Matches(str);

        foreach (var match in matches.ToArray())
        {
            sequences.Add(new Sequence()
            {
                StartIndex = match.Index,
                EndIndex = match.Index + match.Length,
                Chars = match.Value.ToCharArray().ToList()
            });
        }

        return sequences;
    }

    public List<Sequence> BreakIntoWordSequences(Regex allowedChars)
    {
        var sequences = new List<Sequence>();
        var word = new Sequence();
        var isWorkingOnWord = false;

        Iterate((character, index, stop) =>
        {
            var isWordChar = allowedChars.IsMatch($"{character}");

            // middle of the word
            if (isWorkingOnWord && isWordChar)
            {
                isWorkingOnWord = true;
                word.Chars.Add(character);
            }
            // end of the word
            else if (isWorkingOnWord && !isWordChar)
            {
                isWorkingOnWord = false;
                word.EndIndex = index;
                sequences.Add(word.DeepCopy());
                word.Chars.Clear();
            }
            // beginning of the word
            else if (!isWorkingOnWord && isWordChar)
            {
                isWorkingOnWord = true;
                word.StartIndex = index;
                word.Chars.Add(character);
            }
            // character does ont contain a word char
            else
            {
                isWorkingOnWord = false;
            }
            // end of string
            if (index == str.Length - 1 && isWorkingOnWord)
            {
                word.EndIndex = index;
                sequences.Add(word.DeepCopy());
            }
        });

        return sequences;
    }

    public Sequence? FindPattern(string pattern)
    {
        if (pattern.IsEmpty())
        {
            throw new InvalidOperationException("Pattern cannot be an empty string.");
        }
        if (pattern.Length > str.Length)
        {
            throw new InvalidOperationException("Cannot find a pattern that is greater or equal than the string searched on.");
        }

        var sequences = BreakIntoSequences(pattern.Length);

        for (int i = 0; i < sequences.Count; i++)
        {
            var sequence = sequences[i];
            var sequenceString = sequence.ToString();

            if (sequenceString == pattern)
            {
                return sequence;
            }
        }

        return null;
    }

    public Sequence? FindPattern(List<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            var sequence = FindPattern(pattern);

            if (sequence != null)
            {
                return sequence;
            }
        }

        return null;
    }

    public Sequence? FindPatternBackwards(string pattern)
    {
        if (pattern.IsEmpty())
        {
            throw new InvalidOperationException("Pattern cannot be an empty string.");
        }
        if (pattern.Length > str.Length)
        {
            throw new InvalidOperationException("Cannot find a pattern that is greater or equal than the string searched on.");
        }

        var sequences = BreakIntoSequences(pattern.Length);

        sequences.Reverse();

        for (int i = 0; i < sequences.Count; i++)
        {
            var sequence = sequences[i];
            var sequenceString = sequence.ToString();

            if (sequenceString == pattern)
            {
                return sequence;
            }
        }

        return null;
    }

    public Sequence? FindPatternBackwards(List<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            var sequence = FindPatternBackwards(pattern);

            if (sequence != null)
            {
                return sequence;
            }
        }

        return null;
    }

    public Sequence[] FindAllPatterns(IEnumerable<string> patterns)
    {
        var sequences = new List<Sequence>();

        foreach (var pattern in patterns)
        {
            if (pattern.IsEmpty())
            {
                throw new InvalidOperationException("Pattern cannot be an empty string.");
            }
            if (pattern.Length >= str.Length)
            {
                throw new InvalidOperationException("Cannot find a pattern that is greater or equal than the string searched on.");
            }

            var subSequences = BreakIntoSequences(pattern.Length);

            for (int i = 0; i < subSequences.Count; i++)
            {
                var sequence = subSequences[i];
                var sequenceString = sequence.ToString();

                if (sequenceString == pattern)
                {
                    sequences.Add(sequence);
                }
            }
        }

        return sequences.ToArray();
    }

    public Sequence[] FindAllPatterns(string pattern)
    {
        return FindAllPatterns(new string[] { pattern });
    }

    /// <summary>
    /// Finds all the patterns in the string from the end to the start. 
    /// </summary>
    /// <param name="patterns"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Sequence> FindAllPatternsBackwards(List<string> patterns)
    {
        var sequences = new List<Sequence>();

        foreach (var pattern in patterns)
        {
            if (pattern.IsEmpty())
            {
                throw new InvalidOperationException("Pattern cannot be an empty string.");
            }
            if (pattern.Length >= str.Length)
            {
                throw new InvalidOperationException("Cannot find a pattern that is greater or equal than the string searched on.");
            }

            var subSequences = BreakIntoSequences(pattern.Length);

            subSequences.Reverse();

            for (int i = 0; i < subSequences.Count; i++)
            {
                var sequence = subSequences[i];
                var sequenceString = sequence.ToString();

                if (sequenceString == pattern)
                {
                    sequences.Add(sequence);
                }
            }
        }

        return sequences;
    }

    public Sequence? FindRegexPattern(string pattern)
    {
        if (pattern.IsEmpty())
        {
            throw new InvalidOperationException("Pattern cannot be an empty string.");
        }

        var regex = new Regex(pattern);
        var mathes = regex.Matches(pattern);

        if (mathes.Count == 0)
        {
            return null;
        }

        var firstMatch = mathes[0];

        return new Sequence()
        {
            StartIndex = firstMatch.Index,
            EndIndex = firstMatch.Index + firstMatch.Length,
            Chars = firstMatch.ValueSpan.ToArray().ToList(),
        };
    }

    public CutValue Cut(int index, int length = 1)
    {
        if (length <= 0)
        {
            throw new InvalidOperationException("Cannot cut string with a cut length of zero.");
        }
        if (index + length >= str.Length)
        {
            throw new InvalidOperationException("Cannot cut string with a cut length that is greater than the string cut on.");
        }

        var left = str.Substring(0, index);
        var right = str.Substring(index + length, str.Length - (index + length));
        var cutted = new Sequence()
        {
            StartIndex = index,
            EndIndex = index + length,
            Chars = str.Substring(index, index + length).ToList()
        };

        return new CutValue(left, right, cutted);
    }

    public string CutAndGlue(Sequence sequence)
    {
        var cut = CutOn(sequence);
        return cut.Left + cut.Right;
    }

    public string CutAndGlue(IEnumerable<Sequence> sequences)
    {
        List<char> chars = new();

        Iterate((@char, index, stop) =>
        {
            bool isInsideSequence = false;

            foreach (var sequence in sequences)
            {
                if (sequence.StartIndex >= index && sequence.EndIndex <= index + 1)
                {
                    isInsideSequence = true;
                    break;
                }
            }

            if (!isInsideSequence)
            {
                chars.Add(@char);
            }
        });

        return new string(chars.ToArray());
    }

    public CutValue CutOn(Sequence sequence)
    {
        var left = str.Substring(0, sequence.StartIndex);
        var right = str.Substring(sequence.EndIndex, str.Length - sequence.EndIndex);

        return new CutValue(left, right, sequence);
    }

    public bool SequenceIsInsideStringLiteral(Sequence sequence)
    {
        bool isInsideSringLiteral = false;

        Iterate((@char, index, stop) =>
        {
            bool skip = false;

            if (isInsideSringLiteral && @char == '"' && !skip)
            {
                isInsideSringLiteral = false;
                skip = true;
            }
            if (!isInsideSringLiteral && @char == '"' && !skip)
            {
                isInsideSringLiteral = true;
                skip = true;
            }
            if (index == sequence.EndIndex - 1)
            {
                stop();
            }
        });

        return isInsideSringLiteral;
    }

    public string RemoveWhitespaces(bool include_inside_string_literals = false)
    {
        var whitespaces = new List<Sequence>();

        FindAllPatterns(" ")?.ToList().ForEach(x =>
        {
            if (include_inside_string_literals || !SequenceIsInsideStringLiteral(x))
            {
                whitespaces.Add(x);
            }
        });

        return CutAndGlue(whitespaces);
    }
}