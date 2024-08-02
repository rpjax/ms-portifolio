using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Aidan.Webql.Analysis;
using Aidan.Webql.Analysis.Semantics;
using Aidan.Webql.Components;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis.Compilation.LINQ;

public class TranslationContext
{
    public SymbolProduction Production { get; }
    public SemanticsTable SemanticsTable { get; }
    public TranslationTable TranslationTable { get; }

    public void CreateTranslationTableEntry(
           string identifier,
           Expression expression,
           SymbolAccessMode accessMode = SymbolAccessMode.ReadWrite
       )
    {
        if (TranslationTable.ContainsKey(identifier))
        {
            throw new Exception();
        }

        TranslationTable.AddEntry(identifier, expression, accessMode);
    }

    public void CreateOrUpdateTranslationTableEntry(
        string identifier,
        Expression expression,
        SymbolAccessMode accessMode = SymbolAccessMode.ReadWrite
    )
    {
        var entry = TranslationTable.TryGetEntry(identifier);

        if (entry != null)
        {
            if (entry.AccessMode != SymbolAccessMode.ReadWrite)
            {
                throw new Exception();
            }

            entry.Expression = expression;
            return;
        }

        TranslationTable.AddEntry(identifier, expression, accessMode);
    }

    public TranslationTableEntry GetTranslationTableEntry(string identifier)
    {
        var entry = TranslationTable.TryGetEntry(identifier);

        if (entry == null)
        {
            throw new Exception();
        }

        return entry;
    }

    public T GetSemantics<T>(Symbol symbol) where T : SymbolSemantic
    {
        var semantics = SemanticsTable.TryGetEntryByHash(symbol);

        if (semantics is not T result)
        {
            throw new InvalidOperationException();
        }

        return result;
    }

}

public enum SymbolAccessMode
{
    ReadOnly,
    ReadWrite
}

public class TranslationTableEntry
{
    public string Identifier { get; }
    public Expression Expression { get; set; }
    public SymbolAccessMode AccessMode { get; }

    public TranslationTableEntry(string identifier, Expression value, SymbolAccessMode accessMode)
    {
        Identifier = identifier;
        Expression = value;
        AccessMode = accessMode;
    }
}

public class TranslationTable : TableBase<TranslationTableEntry>
{
    private Dictionary<string, TranslationTableEntry> Table { get; } = new();

    public TranslationTableEntry? TryGetEntry(string identifier)
    {
        if (Table.TryGetValue(identifier, out var entry))
        {
            return entry;
        }

        return null;
    }

    public void AddEntry(
        string identifier,
        Expression expression,
        SymbolAccessMode accessMode = SymbolAccessMode.ReadWrite)
    {
        Table.Add(identifier, new TranslationTableEntry(identifier, expression, accessMode));
    }

    public TranslationTableEntry GetEntry(TranslationContextOld context, string identifier)
    {
        return TryGetEntry(identifier)
            ?? throw new TranslationException("", context);
    }
}
