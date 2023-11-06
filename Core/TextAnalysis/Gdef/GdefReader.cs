using ModularSystem.Core;
using System.Text.RegularExpressions;

namespace ModularSystem.Core.TextAnalysis.Gdef;

/// <summary>
/// Reads my custom ".gdef" file. The GDEF extends the BNF format.
/// </summary>
public class GDefReader
{
    public const string FileExtension = "gdef";

    static readonly string AlternativeOperator = "|";
    static readonly string EpsilonOperator = "ε";
    static readonly string CompilerGenerationOperator = "$";
    static readonly string[] GDefOperators =
    {
        AlternativeOperator,
        CompilerGenerationOperator
    };

    public static GdefFile Read(FileInfo file)
    {
        return new GdefFile(new GDefReader().GetProductionDefinitions(file));
    }

    private Dictionary<string, string> GetProdutions(FileInfo gDefFile)
    {
        if (!gDefFile.Exists)
        {
            throw new FileNotFoundException($"The '.{FileExtension}' file was not found at {gDefFile.FullName}");
        }

        var text = File.ReadAllText(gDefFile.FullName);

        var breakLineSplit = text.Split("\r");

        breakLineSplit = breakLineSplit
            .RemoveWhere(x => x == "\r" || x == "\n")
            .ToArray();

        var productions = new Dictionary<string, string>(breakLineSplit.Length / 2);

        foreach (var item in breakLineSplit)
        {
            var split = item.Split("::=");

            if (split.Length != 2)
            {
                throw new InvalidOperationException();
            }

            var lhs = split[0].Replace("\r", "").Replace("\n", "");
            var rhs = split[1].Replace("\r", "").Replace("\n", "");

            productions.Add(lhs, rhs);
        }

        return productions;
    }

    private IEnumerable<KeyValuePair<string, string>> GetTokenProductions(FileInfo gDefFile)
    {
        var regex = new Regex(@"^\[[a-zA-Z-]+\]\s*$");
        return GetProdutions(gDefFile)
            .Where(x => regex.IsMatch(x.Key));
    }

    public List<TokenizedProduction> GetTokenizedProductions(FileInfo gDefFile)
    {
        var tokenProductions = GetTokenProductions(gDefFile);
        var prods = new List<TokenizedProduction>();

        foreach (var item in tokenProductions)
        {
            var definition = new TokenizedProduction(item.Key.Trim());
            var spaceSplit = item.Value
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());

            definition.Tokens.AddRange(spaceSplit);
            prods.Add(definition);
        }

        return prods;
    }

    private ProductionDefinition GetDefinition(TokenizedProduction tokenizedProduction)
    {
        var fragments = new List<PatternFragment>();

        foreach (var token in tokenizedProduction.Tokens)
        {
            PatternFragment? fragment = null;

            if (token.Contains('"') || token.Contains('\''))
            {
                fragment = new TerminalFragment(token);
            }
            else if (token == "{" || token == "}")
            {
                fragment = new RepetitionDelimiterFragment();
            }
            else if (token.Contains("["))
            {
                fragment = new NonTerminalFragment(token);
            }
            else if (GDefOperators.Contains(token))
            {
                fragment = GetOperatorFragment(token);
            }
            else
            {
                throw new InvalidOperationException($"unknown token type, value: '{token}'.");
            }

            fragments.Add(fragment);
        }

        var patterns= new List<Pattern>();
        var pattern = new Pattern();
        var isInsideRepetition = false;
        var count = 0;

        foreach (var fragment in fragments)
        {
            count++;

            var isLast = count == fragments.Count;

            if (fragment is RepetitionDelimiterFragment)
            {
                isInsideRepetition = !isInsideRepetition;
            }
           
            if (!isInsideRepetition && fragment is AlternativeFragment)
            {
                patterns.Add(pattern);
                pattern = new Pattern();
                continue;
            }

            pattern.Fragments.Add(fragment);

            if(isLast)
            {
                if (isInsideRepetition)
                {
                    throw new InvalidOperationException();
                }

                patterns.Add(pattern);
            }
        }

        return new() { Name = tokenizedProduction.Name, Patterns = patterns };
    }

    private PatternFragment GetOperatorFragment(string token)
    {
        if (token == AlternativeOperator)
        {
            return new AlternativeFragment();
        }
        if(token == EpsilonOperator)
        {
            return new EpsilonFragment();
        }
        if(token == CompilerGenerationOperator)
        {
            return new CompilerGenFragment();
        }

        throw new InvalidOperationException($"unknown operator '{token}'.");
    }

    private List<ProductionDefinition> GetProductionDefinitions(FileInfo gDefFile)
    {
        return GetTokenizedProductions(gDefFile)
            .Transform(x => GetDefinition(x))
            .ToList();
    }
}

public class TokenizedProduction
{
    public string Name { get; }
    public List<string> Tokens { get; } = new();

    public TokenizedProduction(string name)
    {
        Name = name;
    }
}

public abstract class PatternFragment
{

}

public class CompilerGenFragment : PatternFragment
{

}

public class AlternativeFragment : PatternFragment
{

}

public class EpsilonFragment : PatternFragment
{

}

public class RepetitionDelimiterFragment : PatternFragment
{

}

public class TerminalFragment : PatternFragment
{
    public string Value { get; }

    public TerminalFragment(string value)
    {
        Value = value;
    }
}

public class NonTerminalFragment : PatternFragment
{
    public string Value { get; }

    public NonTerminalFragment(string value)
    {
        Value = value;
    }
}

public class RepetitionFragment : PatternFragment
{
    public List<PatternFragment> Fragments { get; } = new();
}

public class Pattern
{
    public List<PatternFragment> Fragments { get; } = new();
}

public class ProductionDefinition
{
    public string Name { get; set; }
    public List<Pattern> Patterns { get; set; } = new();
}

public class GdefFile
{
    public List<ProductionDefinition> Productions { get; }

    public GdefFile(List<ProductionDefinition> productions)
    {
        Productions = productions;
    }
}