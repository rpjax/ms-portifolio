using ModularSystem.Webql.Analysis.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics;

/// <summary>
/// acts as symbol table manager suring semantic analysis.
/// </summary>
public class SemanticContextOld
{
    private SymbolTable SymbolTable { get; set; } = new();

    public SemanticContextOld(SymbolTable? symbolTable = null)
    {
        SymbolTable = symbolTable ?? new SymbolTable();
    }

    public void DeclareSymbol(
        string identifier,
        Symbol symbol,
        Type? type,
        bool createScope)
    {
        SymbolTable.AddEntry(
            identifier: identifier,
            symbol: symbol,
            type: type,
            symbolTable: createScope ? new SymbolTable(SymbolTable) : null
        );
    }

    public void UpdateSymbolDeclaration(
        string identifier,
        Symbol symbol,
        Type? type,
        bool createScope)
    {
        var entry = GetSymbolTableEntry(identifier);
        var symbolTable = entry.SymbolTable;

        if (symbolTable is null)
        {
            symbolTable = new SymbolTable(SymbolTable);
        }

        SymbolTable.UpdateEntry(
            identifier: identifier,
            symbol: symbol,
            type: type,
            symbolTable: createScope ? symbolTable : null
        );
    }

    public SymbolTableEntry GetSymbolTableEntry(string identifier)
    {
        var entry = SymbolTable.TryGetEntry(identifier);

        if (entry is null)
        {
            throw new Exception();
        }

        return entry;
    }

    public void EnterScope(string identifier)
    {
        var entry = SymbolTable.TryGetEntry(identifier);

        if (entry is null)
        {
            throw new Exception();
        }

        if (entry.SymbolTable is null)
        {
            throw new Exception();
        }

        SymbolTable = entry.SymbolTable;
    }

    public void ExitScope()
    {
        if (SymbolTable.ParentTable is null)
        {
            throw new Exception();
        }

        SymbolTable = SymbolTable.ParentTable;
    }

    /*
     * helpers.
     */

    public bool ContainsDeclaration(string identifier)
    {
        return SymbolTable.ContainsEntry(identifier);
    }

    public Type GetSymbolType(string identifier)
    {
        var entry = GetSymbolTableEntry(identifier);

        if (entry.Type is null)
        {
            throw new Exception();
        }

        return entry.Type;
    }

    public void CreateScopeForSymbol(string identifier)
    {
        var entry = GetSymbolTableEntry(identifier);

        if (entry.SymbolTable is not null)
        {
            throw new Exception();
        }

        SymbolTable.UpdateEntry(
            identifier: identifier,
            symbol: entry.Symbol,
            type: entry.Type,
            symbolTable: new SymbolTable(SymbolTable)
        );
    }

    public SemanticContextOld GetSymbolContext(ISymbol symbol)
    {
        var entry = SymbolTable.TryGetEntry(symbol.GetIdentifier());

        if (entry?.SymbolTable is null)
        {
            return this;
        }

        return new SemanticContextOld(entry.SymbolTable);
    }

}
